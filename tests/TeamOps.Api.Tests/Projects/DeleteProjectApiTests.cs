using System.Net;
using System.Net.Http.Json;
using TeamOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TeamOps.Api.Tests.Projects;

public class DeleteProjectApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public DeleteProjectApiTests(TeamOpsApiFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task DeleteProject_ShouldRemoveProject()
    {
        // create project

        var tenantId = Guid.NewGuid();

        var request = new
        {
            TenantId = tenantId,
            Name = "Project Alpha"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);
        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(tenantId, project.TenantId);
        Assert.Equal("Project Alpha", project.Name);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedProject = await context.Projects.SingleAsync(x => x.Id == project!.Id);

        Assert.Equal(project.TenantId, savedProject.TenantId);
        Assert.Equal(project.Name, savedProject.Name);


        var deleteResponse = await _client.DeleteAsync($"/api/projects/{project.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // verify project no longer exists in PostgreSQL
        var deletedProject = context.Projects.SingleOrDefault(x => x.Id == project.Id);

        Assert.Null(deletedProject);
    }
}