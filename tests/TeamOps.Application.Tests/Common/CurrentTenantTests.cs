using TeamOps.Application.Common;

namespace TeamOps.Application.Tests.Common;

public sealed class CurrentTenantTests
{
    [Fact]
    public void TenantId_ShouldReturnConfiguredTenant()
    {
        var tenantId = Guid.NewGuid();

        ICurrentTenant currentTenant = new TestCurrentTenant(tenantId);

        Assert.Equal(tenantId, currentTenant.TenantId);
    }

    private sealed class TestCurrentTenant(Guid tenantId) : ICurrentTenant
    {
        public Guid TenantId => tenantId;
    }
}