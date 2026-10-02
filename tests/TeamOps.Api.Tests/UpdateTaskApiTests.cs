using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Api.Tests;

public sealed class UpdateTaskApiTests(
    TeamOpsApiFactory factory)
    : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    [Fact]
    public async Task UpdateTask_ShouldPersistNewTitle()
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

        var createResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Original title"
            });

        var task = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        var response = await _client.PutAsJsonAsync(
            $"/api/tasks/{task.Id}",
            new
            {
                Title = "Updated title",
                TaskId = task.Id
            });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedTask = await context.Tasks.SingleAsync(x => x.Id == task.Id);

        Assert.Equal("Updated title", savedTask.Title);
        Assert.Equal(TaskStatus.Pending, savedTask.Status);
    }

    [Fact]
    public async Task UpdateTask_WhenTaskDoesNotExist_ShouldReturnNotFound()
    {
        var taskId = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync(
            $"/api/tasks/{taskId}",
            new
            {
                Title = "Updated title",
                TaskId = taskId
            });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_WhenTitleIsEmpty_ShouldReturnBadRequest()
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

        var createResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Original title"
            });

        var task = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        var response = await _client.PutAsJsonAsync(
            $"/api/tasks/{task.Id}",
            new
            {
                Title = ""
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}