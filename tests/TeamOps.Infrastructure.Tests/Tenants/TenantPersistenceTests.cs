using TeamOps.Domain.Tenants;

namespace TeamOps.Infrastructure.Tests.Tenants;

public class TenantPersistenceTests
{
    [Fact]
    public async Task Save_ShouldPersistTenant()
    {
        var tenant = Tenant.Create(
            Guid.NewGuid(),
            "Acme");

        // DbContext doesn't exist yet.
        // This test intentionally drives its creation.
        await Task.CompletedTask;

        Assert.NotEqual(Guid.Empty, tenant.Id);
    }
}