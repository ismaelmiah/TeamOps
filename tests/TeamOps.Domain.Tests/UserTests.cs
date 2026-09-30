using TeamOps.Domain.Users;

namespace TeamOps.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUser_WhenValidDataProvided()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var user = User.Create(id, tenantId, "john@example.com", "John");

        Assert.Equal(id, user.Id);
        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("John", user.Name);
    }

    [Fact]
    public void Create_WithEmptyTenantId_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.Empty;

        var act = () => User.Create(id, tenantId, "john@example.com", "John");

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldThrow()
    {
        var act = () => User.Create(
            Guid.Empty,
            Guid.NewGuid(),
            "john@example.com",
            "John");

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var email = string.Empty;

        var act = () => User.Create(id, tenantId, email, "John");

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithWhitespaceEmail_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var email = "   ";

        var act = () => User.Create(id, tenantId, email, "John");

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = string.Empty;

        var act = () => User.Create(id, tenantId, "john@example.com", name);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = "   ";

        var act = () => User.Create(id, tenantId, "john@example.com", name);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_ShouldTrimEmailAndName()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var email = "  john@example.com  ";
        var name = "  John  ";

        var user = User.Create(id, tenantId, email, name);

        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("John", user.Name);
    }

    [Fact]
    public void Create_WithNameExceedingMaximumLength_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = new string('A', 101); // Assuming maximum length is 100

        var act = () => User.Create(id, tenantId, "john@example.com", name);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmailExceedingMaximumLength_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var email = new string('A', 201) + "@example.com"; // Assuming maximum length is 200

        var act = () => User.Create(id, tenantId, email, "John");

        Assert.Throws<ArgumentException>(act);
    }
}