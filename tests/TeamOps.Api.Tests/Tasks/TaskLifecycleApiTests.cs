using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Infrastructure.Persistence;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Api.Tests.Tasks;

public class TaskLifecycleApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public TaskLifecycleApiTests(TeamOpsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StartTask_ShouldMoveTaskToInProgress()
    {
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", Guid.NewGuid().ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var projectResponse = await _client.SendAsync(createRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        var taskResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Implement authentication"
            });

        var task = await taskResponse.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        var response = await _client.PostAsync($"/api/tasks/{task.Id}/start", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Equal(TaskStatus.InProgress, savedTask.Status);
    }

    [Fact]
    public async Task HoldTask_ShouldMoveTaskToOnHold()
    {
        var task = await CreateTaskAsync();

        var response = await _client.PostAsync($"/api/tasks/{task.Id}/hold", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Equal(TaskStatus.OnHold, savedTask.Status);
    }

    [Fact]
    public async Task CompleteTask_ShouldMoveInProgressTaskToCompleted()
    {
        var task = await CreateTaskAsync();

        var startResponse = await _client.PostAsync($"/api/tasks/{task.Id}/start", null);

        Assert.Equal(HttpStatusCode.NoContent, startResponse.StatusCode);

        var completeResponse = await _client.PostAsync($"/api/tasks/{task.Id}/complete", null);

        Assert.Equal(HttpStatusCode.NoContent, completeResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Equal(TaskStatus.Completed, savedTask.Status);
    }

    private async Task<TaskResponse> CreateTaskAsync()
    {
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", Guid.NewGuid().ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var projectResponse = await _client.SendAsync(createRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        var taskResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Test task"
            });

        Assert.Equal(HttpStatusCode.Created, taskResponse.StatusCode);

        var task = await taskResponse.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        return task;
    }
}