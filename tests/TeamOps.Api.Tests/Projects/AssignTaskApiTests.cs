using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Domain.Users;
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

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Jane Smith",
            Role = UserRole.Member
        });

        var userResponse = await _client.SendAsync(request);

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

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", userTenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Jane Smith",
            Role = UserRole.Member
        });

        var userResponse = await _client.SendAsync(request);

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

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Null(savedTask.AssigneeId);
    }

    [Fact]
    public async Task UnassignTask_ShouldRemoveAssignee()
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

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Jane Smith",
            Role = UserRole.Member
        });

        var userResponse = await _client.SendAsync(request);

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


        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{task.Id}/assignee");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Null(savedTask.AssigneeId);
    }

    [Fact]
    public async Task UnassignTask_WhenTaskDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync($"/api/tasks/{Guid.NewGuid()}/assignee");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}