using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Infrastructure.Persistence;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Api.Tests.Tasks;

public class CreateTaskApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public CreateTaskApiTests(TeamOpsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_ShouldPersistTask()
    {
        // Create project first.
        var tenantId = Guid.NewGuid();

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var projectResponse = await _client.SendAsync(createRequest);
        projectResponse.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create task.
        using var taskRequest = new HttpRequestMessage(HttpMethod.Post, "/api/tasks");

        taskRequest.Headers.Add("X-Test-Tenant", project.TenantId.ToString());
        taskRequest.Content = JsonContent.Create(new
        {
                ProjectId = project.Id,
                Title = "Implement authentication"
        });

        var response = await _client.SendAsync(taskRequest);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);
        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(project.Id, task.ProjectId);
        Assert.Equal("Implement authentication", task.Title);

        // Verify persistence.
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Equal(project.Id, savedTask.ProjectId);
        Assert.Equal("Implement authentication", savedTask.Title);
    }
    [Fact]
    public async Task CreateTask_ShouldStartAsPending()
    {
        var tenantId = Guid.NewGuid();
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var projectResponse = await _client.SendAsync(createRequest);
        projectResponse.EnsureSuccessStatusCode();

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        using var taskRequest = new HttpRequestMessage(HttpMethod.Post, "/api/tasks");

        taskRequest.Headers.Add("X-Test-Tenant", project.TenantId.ToString());
        taskRequest.Content = JsonContent.Create(new
        {
                ProjectId = project.Id,
                Title = "Implement authentication"
        });

        var response = await _client.SendAsync(taskRequest);
        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Equal(TaskStatus.Pending, savedTask.Status);
    }

    [Fact]
    public async Task CreateTask_WithEmptyTitle_ShouldReturnBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = Guid.NewGuid(),
                Title = ""
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithWhitespaceTitle_ShouldReturnBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = Guid.NewGuid(),
                Title = "   "
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}