using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Api.Tests.Projects;

public class RemoveProjectMemberApiTests
    : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public RemoveProjectMemberApiTests(
        TeamOpsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RemoveMember_ShouldRemoveUserFromProject()
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

        Assert.Equal(
            HttpStatusCode.Created,
            projectResponse.StatusCode);

        var project = await projectResponse.Content
            .ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create user.
        var userResponse = await _client.PostAsJsonAsync(
            "/api/users",
            new
            {
                TenantId = tenantId,
                Email = $"user-{Guid.NewGuid()}@example.com",
                Name = "John Smith",
                Role = 0
            });

        Assert.Equal(
            HttpStatusCode.Created,
            userResponse.StatusCode);

        var user = await userResponse.Content
            .ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Add user to project.
        var addResponse = await _client.PostAsync(
            $"/api/projects/{project.Id}/members/{user.Id}",
            content: null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            addResponse.StatusCode);

        // Remove user from project.
        var removeResponse = await _client.DeleteAsync(
            $"/api/projects/{project.Id}/members/{user.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            removeResponse.StatusCode);

        // Verify membership was removed from PostgreSQL.
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<TeamOpsDbContext>();

        var membership = await context.ProjectMembers
            .SingleOrDefaultAsync(x =>
                x.ProjectId == project.Id &&
                x.UserId == user.Id);

        Assert.Null(membership);
    }

    [Fact]
    public async Task RemoveMember_WhenUserIsNotMember_ShouldReturnNotFound()
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

        // Create user but do NOT add them to the project.
        var userResponse = await _client.PostAsJsonAsync(
            "/api/users",
            new
            {
                TenantId = tenantId,
                Email = $"user-{Guid.NewGuid()}@example.com",
                Name = "Jane Smith",
                Role = 0
            });

        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Try to remove a user who isn't a member.
        var response = await _client.DeleteAsync($"/api/projects/{project.Id}/members/{user.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveMember_WhenUserBelongsToDifferentTenant_ShouldReturnNotFound()
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
        var userResponse = await _client.PostAsJsonAsync(
            "/api/users",
            new
            {
                TenantId = userTenantId,
                Email = $"user-{Guid.NewGuid()}@example.com",
                Name = "Jane Smith",
                Role = 0
            });

        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        var response = await _client.DeleteAsync($"/api/projects/{project.Id}/members/{user.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}