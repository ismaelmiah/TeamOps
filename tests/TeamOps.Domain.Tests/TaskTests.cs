using TeamOps.Domain.Tasks;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Domain.Tests;
public class TaskTests
{
    [Fact]
    public void Create_WithValidProjectId_ShouldCreateTask()
    {
        // Arrange
        var id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var title = "Implement login";

        // Act
        var task = TaskItem.Create(id, projectId, title);

        // Assert
        Assert.Equal(id, task.Id);
        Assert.Equal(projectId, task.ProjectId);
        Assert.Equal(title, task.Title);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var title = string.Empty;

        // Act
        var act = () => TaskItem.Create(id, projectId, title);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithTitleContainingSurroundingWhitespace_ShouldTrimTitle()
    {
        // Arrange
        var id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var title = "  Implement login  ";

        // Act
        var task = TaskItem.Create(id, projectId, title);

        // Assert
        Assert.Equal("Implement login", task.Title);
    }

    [Fact]
    public void Create_ShouldSetStatusToPending()
    {
        // Arrange
        var id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var title = "Implement login";

        // Act
        var task = TaskItem.Create(id, projectId, title);

        // Assert
        Assert.Equal(TaskStatus.Pending, task.Status);
    }

    [Fact]
    public void Start_ShouldChangeStatusToInProgress()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        task.Start();

        Assert.Equal(TaskStatus.InProgress, task.Status);
    }

    [Fact]
    public void Hold_ShouldChangeStatusToOnHold()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");
        task.Start();

        task.Hold();

        Assert.Equal(TaskStatus.OnHold, task.Status);
    }

    [Fact]
    public void Complete_ShouldChangeStatusToCompleted()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");
        task.Start();

        task.Complete();

        Assert.Equal(TaskStatus.Completed, task.Status);
    }

    [Fact]
    public void Start_ShouldThrow_WhenTaskIsCompleted()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        task.Start();
        task.Complete();

        Assert.Throws<InvalidOperationException>(() => task.Start());
    }

    [Fact]
    public void Complete_ShouldThrow_WhenTaskIsOnHold()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        task.Hold();

        Assert.Throws<InvalidOperationException>(() => task.Complete());
    }

    [Fact]
    public void Complete_ShouldThrow_WhenTaskIsPending()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        Assert.Throws<InvalidOperationException>(() => task.Complete());
    }


    [Fact]
    public void Hold_ShouldThrow_WhenTaskIsCompleted()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");
        task.Start();
        task.Complete();

        Assert.Throws<InvalidOperationException>(() => task.Hold());
    }
}