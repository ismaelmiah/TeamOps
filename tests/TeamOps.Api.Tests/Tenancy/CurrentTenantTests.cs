using System.Net;
using System.Net.Http.Json;
using TeamOps.Domain.Users;

namespace TeamOps.Api.Tests.Tenancy;

public sealed class CurrentTenantTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;

    public CurrentTenantTests(TeamOpsApiFactory factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Request_with_tenant_identity_returns_current_tenant()
    {
        var tenantId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/tenant");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains(tenantId.ToString(), content);
    }

    [Fact]
    public async Task Create_user_uses_tenant_from_authenticated_identity()
    {
        var tenantId = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Test User",
            role = UserRole.Member
        });

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains(tenantId.ToString(), content);
    }
}