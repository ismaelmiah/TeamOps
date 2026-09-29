using System.Net;
using System.Net.Http.Json;

namespace TeamOps.Api.Tests.Projects;

public class GetProjectApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;

    public GetProjectApiTests(TeamOpsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ShouldReturnProject()
    {
        var tenantId = Guid.NewGuid();

        var createRequest = new
        {
            TenantId = tenantId,
            Name = "Project Alpha"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/projects", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(createdProject);

        var response = await _client.GetAsync($"/api/projects/{createdProject.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);
        Assert.Equal(createdProject.Id, project.Id);
        Assert.Equal(tenantId, project.TenantId);
        Assert.Equal("Project Alpha", project.Name);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        var projectId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/projects/{projectId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record ProjectResponse(
        Guid Id,
        Guid TenantId,
        string Name);
}