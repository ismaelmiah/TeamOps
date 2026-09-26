using TeamOps.Domain.Projects;

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
}