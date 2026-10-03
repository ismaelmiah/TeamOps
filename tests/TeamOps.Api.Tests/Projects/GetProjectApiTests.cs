using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

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

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(createdProject);

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/projects/{createdProject.Id}");

        getRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());

        var getResponse = await _client.SendAsync(getRequest);
        getResponse.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var project = await getResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);
        Assert.Equal(createdProject.Id, project.Id);
        Assert.Equal(tenantId, project.TenantId);
        Assert.Equal("Project Alpha", project.Name);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        var projectId = Guid.NewGuid();

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/projects/{projectId}");

        getRequest.Headers.Add("X-Test-Tenant", Guid.NewGuid().ToString());

        var response = await _client.SendAsync(getRequest);

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

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/projects");

        getRequest.Headers.Add("X-Test-Tenant", tenantA.ToString());
        var response = await _client.SendAsync(getRequest);
        response.EnsureSuccessStatusCode();

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
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = name
        });

        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    }

    [Fact]
    public async Task Get_project_from_another_tenant_returns_not_found()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        // Create project as tenant A
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", tenantA.ToString());

        createRequest.Content = JsonContent.Create(new
        {
            name = "Tenant A Project"
        });

        var createResponse = await _client.SendAsync(createRequest);

        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();

        var projectId = created.GetProperty("id").GetGuid();

        // Try to access it as tenant B
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/projects/{projectId}");

        getRequest.Headers.Add("X-Test-Tenant", tenantB.ToString());

        var getResponse = await _client.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}