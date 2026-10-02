using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Domain.Users;
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

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "john@example.com",
            name = "John Smith",
            Role = UserRole.Member
        });

        var response = await _client.SendAsync(request);
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
}