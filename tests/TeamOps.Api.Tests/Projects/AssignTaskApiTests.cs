using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Api.Tests.Tasks;

public class AssignTaskApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public AssignTaskApiTests(TeamOpsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AssignTask_ShouldAssignUser()
    {
        var tenantId = Guid.NewGuid();

        var projectResponse = await _client.PostAsJsonAsync(
            "/api/projects",
            new
            {
                TenantId = tenantId,
                Name = "Project Alpha"
            });

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        var userResponse = await _client.PostAsJsonAsync(
            "/api/users",
            new
            {
                TenantId = tenantId,
                Email = "john@example.com",
                Name = "John",
                Role = 0
            });

        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        var taskResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Implement authentication"
            });

        var task = await taskResponse.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        var response = await _client.PostAsync(
            $"/api/projects/{project.Id}/tasks/{task.Id}/assignee/{user.Id}",
            null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Equal(user.Id, savedTask.AssigneeId);
    }

    [Fact]
    public async Task AssignTask_WhenUserBelongsToDifferentTenant_ShouldReturnBadRequest()
    {
        var projectTenantId = Guid.NewGuid();
        var userTenantId = Guid.NewGuid();

        var projectResponse = await _client.PostAsJsonAsync(
            "/api/projects",
            new
            {
                TenantId = projectTenantId,
                Name = "Project Alpha"
            });

        var project = await projectResponse.Content
            .ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        var userResponse = await _client.PostAsJsonAsync(
            "/api/users",
            new
            {
                TenantId = userTenantId,
                Email = "other@example.com",
                Name = "Other User",
                Role = 0
            });

        var user = await userResponse.Content
            .ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        var taskResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Implement authentication"
            });

        var task = await taskResponse.Content
            .ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        var response = await _client.PostAsync(
            $"/api/projects/{project.Id}/tasks/{task.Id}/assignee/{user.Id}",
            null);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks
            .SingleAsync(x => x.Id == task.Id);

        Assert.Null(savedTask.AssigneeId);
    }

    private sealed record ProjectResponse(
        Guid Id,
        Guid TenantId,
        string Name);

    private sealed record UserResponse(
        Guid Id,
        Guid TenantId,
        string Email,
        string Name,
        int Role);

    private sealed record TaskResponse(
        Guid Id,
        Guid ProjectId,
        string Title);
}