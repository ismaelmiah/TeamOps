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

        using var taskRequest = new HttpRequestMessage(HttpMethod.Post, "/api/tasks");

        taskRequest.Headers.Add("X-Test-Tenant", project.TenantId.ToString());
        taskRequest.Content = JsonContent.Create(new
        {
                ProjectId = project.Id,
                Title = "Implement authentication"
        });

        var createResponse = await _client.SendAsync(taskRequest);
        createResponse.EnsureSuccessStatusCode();

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
}