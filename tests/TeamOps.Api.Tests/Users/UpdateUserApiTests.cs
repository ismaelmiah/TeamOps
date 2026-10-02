using System.Net;
using System.Net.Http.Json;
using TeamOps.Domain.Users;

namespace TeamOps.Api.Tests.Users;

public sealed class UpdateUserApiTests(TeamOpsApiFactory factory) : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UpdateUser_ShouldPersistChanges()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user_a@example.com",
            name = "Test user",
            Role = UserRole.Member
        });

        var createResponse = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var user = await createResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/users/{user.Id}",
            new
            {
                Email = "updated@example.com",
                Name = "Updated Name",
                Role = UserRole.Admin
            });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert
        var getResponse = await _client.GetAsync($"/api/users/{user.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updatedUser = await getResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(updatedUser);
        Assert.Equal(user.Id, updatedUser.Id);
        Assert.Equal(tenantId, updatedUser.TenantId);
        Assert.Equal("updated@example.com", updatedUser.Email);
        Assert.Equal("Updated Name", updatedUser.Name);
        Assert.Equal(UserRole.Admin, updatedUser.Role);
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/users/{Guid.NewGuid()}",
            new
            {
                Email = "updated@example.com",
                Name = "Updated Name",
                Role = UserRole.Admin
            });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_WhenEmailIsEmpty_ShouldReturnBadRequest()
    {
        var tenantId = Guid.NewGuid();
        
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");
        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user_a@example.com",
            name = "Test user",
            Role = UserRole.Member
        });

        var createResponse = await _client.SendAsync(request);
        var user = await createResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        var response = await _client.PutAsJsonAsync(
            $"/api/users/{user.Id}",
            new
            {
                Email = "",
                Name = "Updated Name",
                Role = UserRole.Member
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_WhenNameIsEmpty_ShouldReturnBadRequest()
    {
        var tenantId = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");
        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user_a@example.com",
            name = "Test user",
            Role = UserRole.Member
        });

        var createResponse = await _client.SendAsync(request);
        var user = await createResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        var response = await _client.PutAsJsonAsync(
            $"/api/users/{user.Id}",
            new
            {
                Email = "updated@example.com",
                Name = "",
                Role = UserRole.Member
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}