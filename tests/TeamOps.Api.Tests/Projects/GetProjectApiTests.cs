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


    [Fact]
    public async Task Get_ShouldReturnOnlyProjectsForTenant()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await CreateProject(tenantA, "Project A1");
        await CreateProject(tenantA, "Project A2");
        await CreateProject(tenantB, "Project B1");

        var response = await _client.GetAsync($"/api/projects?tenantId={tenantA}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectResponse>>();

        Assert.NotNull(projects);

        Assert.Equal(2, projects.Count);
        Assert.All(projects, project => Assert.Equal(tenantA, project.TenantId));

        Assert.Contains(projects, project => project.Name == "Project A1");
        Assert.Contains(projects, project => project.Name == "Project A2");
        Assert.DoesNotContain(projects, project => project.Name == "Project B1");
    }

    private async Task CreateProject(Guid tenantId, string name)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/projects",
            new
            {
                TenantId = tenantId,
                Name = name
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    private sealed record ProjectResponse(
        Guid Id,
        Guid TenantId,
        string Name);
}