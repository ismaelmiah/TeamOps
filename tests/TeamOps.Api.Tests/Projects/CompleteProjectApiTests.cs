using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Domain.Projects;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Api.Tests.Projects;

public class CompleteProjectApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public CompleteProjectApiTests(TeamOpsApiFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task Complete_ShouldCompleteProject()
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

        var response = await _client.PostAsync($"/api/projects/{createdProject.Id}/complete", content: null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedProject = await context.Projects.SingleAsync(x => x.Id == createdProject.Id);

        Assert.Equal(ProjectStatus.Completed, savedProject.Status);
    }
}