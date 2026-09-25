using TeamOps.Domain.Tenants;

namespace TeamOps.Domain.Tests;

public class TenantTests
{
    [Fact]
    public void Create_WithValidName_ShouldCreateTenant()
    {
        // Arrange
        var name = "Acme Ltd";

        // Act
        var tenant = Tenant.Create(name);

        // Assert
        Assert.Equal(name, tenant.Name);
    }
}