using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Domain.Users;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Api.Tests.Projects;

public class AddProjectMemberApiTests
    : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public AddProjectMemberApiTests(TeamOpsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddMember_ShouldAddUserToProject()
    {
        var tenantId = Guid.NewGuid();

        // Create project.
        var projectResponse = await _client.PostAsJsonAsync(
            "/api/projects",
            new
            {
                TenantId = tenantId,
                Name = "Project Alpha"
            });

        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create user in the same tenant.
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Test User",
            Role = UserRole.Member
        });

        var userResponse = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Add user to project.
        var response = await _client.PostAsync(
            $"/api/projects/{project.Id}/members/{user.Id}",
            content: null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<TeamOpsDbContext>();

        var membership = await context.ProjectMembers
            .SingleAsync(x =>
                x.ProjectId == project.Id &&
                x.UserId == user.Id);

        Assert.Equal(project.Id, membership.ProjectId);
        Assert.Equal(user.Id, membership.UserId);
    }

    [Fact]
    public async Task AddMember_WhenUserBelongsToDifferentTenant_ShouldReturnBadRequest()
    {
        var projectTenantId = Guid.NewGuid();
        var userTenantId = Guid.NewGuid();

        // Create project in Tenant A.
        var projectResponse = await _client.PostAsJsonAsync(
            "/api/projects",
            new
            {
                TenantId = projectTenantId,
                Name = "Project Alpha"
            });

        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create user in Tenant B.

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", userTenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Jane Smith",
            Role = UserRole.Member
        });

        var userResponse = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Try to add Tenant B user to Tenant A project.
        var response = await _client.PostAsync(
            $"/api/projects/{project.Id}/members/{user.Id}",
            content: null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<TeamOpsDbContext>();

        var membership = await context.ProjectMembers
            .SingleOrDefaultAsync(x =>
                x.ProjectId == project.Id &&
                x.UserId == user.Id);

        Assert.Null(membership);
    }
}