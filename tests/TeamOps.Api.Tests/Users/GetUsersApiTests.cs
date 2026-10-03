using System.Net;
using System.Net.Http.Json;
using TeamOps.Domain.Users;

namespace TeamOps.Api.Tests.Users;

public sealed class GetUsersApiTests(TeamOpsApiFactory factory) : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetUser_ShouldReturnUser()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Test User",
            Role = UserRole.Member
        });

        var createResponse = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(createdUser);

        // Act
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{createdUser.Id}");
        getRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        var response = await _client.SendAsync(getRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);
        Assert.Equal(createdUser.Id, user.Id);
        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal("Test User", user.Name);
        Assert.Equal(UserRole.Member, user.Role);
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{Guid.NewGuid()}");
        getRequest.Headers.Add("X-Test-Tenant", Guid.NewGuid().ToString());
        var response = await _client.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetUsersByTenant_ShouldReturnOnlyUsersFromTenant()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        using var requestA = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        requestA.Headers.Add("X-Test-Tenant", tenantA.ToString());

        requestA.Content = JsonContent.Create(new
        {
            email = "user-a@example.com",
            name = "User A",
            Role = UserRole.Member
        });

        var userAResponse = await _client.SendAsync(requestA);
        Assert.Equal(HttpStatusCode.Created, userAResponse.StatusCode);

        using var requestB = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        requestB.Headers.Add("X-Test-Tenant", tenantB.ToString());

        requestB.Content = JsonContent.Create(new
        {
            email = "user_b@example.com",
            name = "User B",
            Role = UserRole.Member
        });

        var userBResponse = await _client.SendAsync(requestB);

        Assert.Equal(HttpStatusCode.Created, userBResponse.StatusCode);

        // Act

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users");
        getRequest.Headers.Add("X-Test-Tenant", tenantA.ToString());
        var response = await _client.SendAsync(getRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();

        Assert.NotNull(users);

        Assert.Single(users);

        Assert.Equal(tenantA, users[0].TenantId);

        Assert.Equal("user-a@example.com", users[0].Email);
    }

    [Fact]
    public async Task GetUsersByTenant_WhenTenantHasNoUsers_ShouldReturnEmptyList()
    {
        var tenantId = Guid.NewGuid();

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users");
        getRequest.Headers.Add("X-Test-Tenant", tenantId.ToString());
        var response = await _client.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();

        Assert.NotNull(users);
        Assert.Empty(users);
    }
}