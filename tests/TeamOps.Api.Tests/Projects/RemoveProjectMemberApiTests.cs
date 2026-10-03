using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Domain.Users;
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
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var projectResponse = await _client.SendAsync(createRequest);
        projectResponse.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create user.

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

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

        // Add user to project.
        using var addRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/projects/{project.Id}/members/{user.Id}");

        addRequest.Headers.Add("X-Test-Tenant", project.TenantId.ToString());
        var addResponse = await _client.SendAsync(addRequest);
        addResponse.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.NoContent, addResponse.StatusCode);

        // Remove user from project.
        var removeResponse = await _client.DeleteAsync(
            $"/api/projects/{project.Id}/members/{user.Id}");

        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        // Verify membership was removed from PostgreSQL.
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

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
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var projectResponse = await _client.SendAsync(createRequest);
        projectResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create user but do NOT add them to the project.
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

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
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/projects");

        createRequest.Headers.Add("X-Test-Tenant", projectTenantId.ToString());
        createRequest.Content = JsonContent.Create(new
        {
            Name = "Project Alpha"
        });

        var projectResponse = await _client.SendAsync(createRequest);
        projectResponse.EnsureSuccessStatusCode();
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

        var response = await _client.DeleteAsync($"/api/projects/{project.Id}/members/{user.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}