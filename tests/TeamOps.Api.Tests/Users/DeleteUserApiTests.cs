using System.Net;
using System.Net.Http.Json;
using TeamOps.Domain.Users;

namespace TeamOps.Api.Tests.Users;

public sealed class DeleteUserApiTests(TeamOpsApiFactory factory) : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser()
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

        var user = await createResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Act
        var response = await _client.DeleteAsync($"/api/users/{user.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/users/{user.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync($"/api/users/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}