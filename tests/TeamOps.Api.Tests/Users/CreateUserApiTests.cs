using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Api.Tests.Users;

public class CreateUserApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public CreateUserApiTests(TeamOpsApiFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task Post_ShouldCreateUser()
    {
        var tenantId = Guid.NewGuid();

        var request = new
        {
            TenantId = tenantId,
            Email = "john@example.com",
            Name = "John Smith",
            Role = 0
        };

        var response = await _client.PostAsJsonAsync(
            "/api/users",
            request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("John Smith", user.Name);

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedUser = await context.Users.SingleAsync(x => x.Id == user.Id);

        Assert.Equal(user.TenantId, savedUser.TenantId);
        Assert.Equal(user.Email, savedUser.Email);
        Assert.Equal(user.Name, savedUser.Name);
        Assert.Equal(user.Role, savedUser.Role);
    }

    private sealed record UserResponse(
        Guid Id,
        Guid TenantId,
        string Email,
        string Name,
        UserRole Role);
}