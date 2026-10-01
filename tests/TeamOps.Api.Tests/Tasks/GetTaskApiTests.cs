using System.Net;
using System.Net.Http.Json;

namespace TeamOps.Api.Tests.Tasks;

public class GetTaskApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;

    public GetTaskApiTests(TeamOpsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTask_ShouldReturnTask()
    {
        var projectResponse = await _client.PostAsJsonAsync(
            "/api/projects",
            new
            {
                TenantId = Guid.NewGuid(),
                Name = "Project Alpha"
            });

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Implement authentication"
            });

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(createdTask);

        var response = await _client.GetAsync($"/api/tasks/{createdTask.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);
        Assert.Equal(createdTask.Id, task.Id);
        Assert.Equal(project.Id, task.ProjectId);
        Assert.Equal("Implement authentication", task.Title);
    }

    [Fact]
    public async Task GetTask_WhenTaskDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record ProjectResponse(
        Guid Id,
        Guid TenantId,
        string Name);

    private sealed record TaskResponse(
        Guid Id,
        Guid ProjectId,
        string Title);
}