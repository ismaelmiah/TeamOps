using System.Net;
using System.Net.Http.Json;

namespace TeamOps.Api.Tests.Projects;

public class AddProjectMemberApiTests
    : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;

    public AddProjectMemberApiTests(TeamOpsApiFactory factory)
    {
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

        Assert.Equal(
            HttpStatusCode.Created,
            projectResponse.StatusCode);

        var project = await projectResponse.Content
            .ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create user in the same tenant.
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
        var response = await _client.PostAsync(
            $"/api/projects/{project.Id}/members/{user.Id}",
            content: null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    private sealed record ProjectResponse(
        Guid Id,
        Guid TenantId,
        string Name);

    private sealed record UserResponse(
        Guid Id,
        Guid TenantId,
        string Email,
        string Name,
        int Role);
}