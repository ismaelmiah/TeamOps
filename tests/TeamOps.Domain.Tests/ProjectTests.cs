using TeamOps.Domain.Projects;
using TeamOps.Domain.Users;

namespace TeamOps.Domain.Tests;

public class ProjectTests
{
    [Fact]
    public void Create_WithValidTenantId_ShouldCreateProject()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = "Website Redesign";

        // Act
        var project = Project.Create(id, tenantId, name);

        // Assert
        Assert.Equal(id, project.Id);
        Assert.Equal(tenantId, project.TenantId);
        Assert.Equal(name, project.Name);
    }

    [Fact]
    public void Create_WithEmptyTenantId_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.Empty;
        var name = "Website Redesign";

        // Act
        var act = () => Project.Create(id, tenantId, name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = string.Empty;

        // Act
        var act = () => Project.Create(id, tenantId, name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithNameContainingSurroundingWhitespace_ShouldTrimName()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = "  Website Redesign  ";

        // Act
        var project = Project.Create(id, tenantId, name);

        // Assert
        Assert.Equal("Website Redesign", project.Name);
    }

    [Fact]
    public void Create_WithNameExceedingMaximumLength_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = new string('A', 101);

        // Act
        var act = () => Project.Create(id, tenantId, name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithNameAtMaximumLength_ShouldCreateProject()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = new string('A', 100);

        // Act
        var project = Project.Create(id, tenantId, name);

        // Assert
        Assert.Equal(name, project.Name);
    }

    [Fact]
    public void Create_ShouldSetStatusToActive()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = "Website Redesign";

        // Act
        var project = Project.Create(id, tenantId, name);

        // Assert
        Assert.Equal(ProjectStatus.Active, project.Status);
    }

    [Fact]
    public void Complete_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website Redesign");

        // Act
        project.Complete();

        // Assert
        Assert.Equal(ProjectStatus.Completed, project.Status);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldThrow()
    {
        // Arrange
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website Redesign");

        project.Complete();

        // Act
        var act = () => project.Complete();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AddMember_WithNullUser_ShouldThrow()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");
        User user = null;

        var act = () => project.AddMember(user);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddMember_ShouldAddUser_WhenUserBelongsToSameTenant()
    {
        var tenantId = Guid.NewGuid();

        var project = Project.Create(
            Guid.NewGuid(),
            tenantId,
            "Website");

        var user = User.Create(
            Guid.NewGuid(),
            tenantId,
            "john@example.com",
            "John");

        project.AddMember(user);

        Assert.Contains(user.Id, project.MemberIds);
    }

    [Fact]
    public void AddMember_ShouldRejectUser_WhenUserBelongsToDifferentTenant()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "john@example.com",
            "John");

        var act = () => project.AddMember(user);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AddMember_ShouldNotAllowDuplicateUser()
    {
        var tenantId = Guid.NewGuid();

        var project = Project.Create(
            Guid.NewGuid(),
            tenantId,
            "Website");

        var user = User.Create(
            Guid.NewGuid(),
            tenantId,
            "john@example.com",
            "John");

        project.AddMember(user);

        var act = () => project.AddMember(user);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void RemoveMember_WithEmptyUserId_ShouldThrow()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var act = () => project.RemoveMember(Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void RemoveMember_ShouldRemoveMemberFromProject()
    {
        var tenantId = Guid.NewGuid();
        var project = Project.Create(
            Guid.NewGuid(),
            tenantId,
            "Website");

        var user = User.Create(
            Guid.NewGuid(),
            tenantId,
            "john@example.com",
            "John");

        project.AddMember(user);
        project.RemoveMember(user.Id);

        Assert.DoesNotContain(user.Id, project.MemberIds);
    }

    [Fact]
    public void RemoveMember_ShouldThrow_WhenUserIsNotMember()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var userId = Guid.NewGuid();

        var act = () => project.RemoveMember(userId);

        Assert.Throws<InvalidOperationException>(act);
    }

}