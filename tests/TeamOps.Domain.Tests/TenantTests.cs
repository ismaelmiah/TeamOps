using TeamOps.Domain.Tenants;

namespace TeamOps.Domain.Tests;

public class TenantTests
{
    [Fact]
    public void Create_WithValidName_ShouldCreateTenant()
    {
        // Arrange

        var id = Guid.NewGuid();
        var name = "Acme Ltd";

        // Act
        var tenant = Tenant.Create(id, name);

        // Assert
        Assert.Equal(name, tenant.Name);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = string.Empty;

        // Act
        var act = () => Tenant.Create(id, name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithNullName_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        string? name = null;

        // Act
        var act = () => Tenant.Create(id, name!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "   ";

        // Act
        var act = () => Tenant.Create(id, name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithValidName_ShouldHaveId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Acme Ltd";

        // Act
        var tenant = Tenant.Create(id, name);

        // Assert
        Assert.Equal(id, tenant.Id);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldThrow()
    {
        // Arrange
        var id = Guid.Empty;
        var name = "Acme Ltd";

        // Act
        var act = () => Tenant.Create(id, name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }


    [Fact]
    public void Create_WithNameContainingSurroundingWhitespace_ShouldTrimName()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "  Acme Ltd  ";

        // Act
        var tenant = Tenant.Create(id, name);

        // Assert
        Assert.Equal("Acme Ltd", tenant.Name);
    }

    [Fact]
    public void Create_WithNameExceedingMaximumLength_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = new string('A', 101);

        // Act
        var act = () => Tenant.Create(id, name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithNameAtMaximumLength_ShouldCreateTenant()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = new string('A', 100);

        // Act
        var tenant = Tenant.Create(id, name);

        // Assert
        Assert.Equal(name, tenant.Name);
    }

    [Fact]
    public void Create_ShouldSetCreatedAt()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Acme Ltd";
        var before = DateTime.UtcNow;

        // Act
        var tenant = Tenant.Create(id, name);

        // Assert
        var after = DateTime.UtcNow;

        Assert.InRange(tenant.CreatedAt, before, after);
    }
}