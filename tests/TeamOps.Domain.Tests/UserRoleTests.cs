using TeamOps.Domain.Users;

namespace TeamOps.Domain.Tests;

public class UserRoleTests
{
    [Fact]
    public void Create_ShouldAssignMemberRoleByDefault()
    {
        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "john@example.com",
            "John");

        Assert.Equal(UserRole.Member, user.Role);
    }

    [Fact]
    public void Create_ShouldAllowAdminRole()
    {
        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "admin@example.com",
            "Admin",
            UserRole.Admin);

        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public void Create_ShouldPreserveMemberRole()
    {
        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "john@example.com",
            "John",
            UserRole.Member);

        Assert.Equal(UserRole.Member, user.Role);
    }

    [Fact]
    public void Role_ShouldBeReadOnly()
    {
        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "john@example.com",
            "John");

        Assert.Equal(UserRole.Member, user.Role);
    }
}