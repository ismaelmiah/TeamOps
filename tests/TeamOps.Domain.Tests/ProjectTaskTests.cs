using TeamOps.Domain.Projects;
using TeamOps.Domain.Tasks;
using TeamOps.Domain.Users;

namespace TeamOps.Domain.Tests;

public class ProjectTaskTests
{
    [Fact]
    public void AddTask_ShouldAddTaskToProject()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        project.AddTask(task);

        Assert.Contains(task.Id, project.TaskIds);
    }

    [Fact]
    public void AddTask_ShouldRejectTaskBelongingToDifferentProject()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        var act = () => project.AddTask(task);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AddTask_ShouldNotAllowDuplicateTask()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        project.AddTask(task);

        var act = () => project.AddTask(task);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void RemoveTask_ShouldRemoveExistingTask()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        project.AddTask(task);
        project.RemoveTask(task.Id);

        Assert.DoesNotContain(task.Id, project.TaskIds);
    }

    [Fact]
    public void RemoveTask_ShouldThrow_WhenTaskIsNotInProject()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var act = () => project.RemoveTask(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AssignTask_ShouldAssignUser_WhenUserBelongsToProjectTenant()
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

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        project.AddTask(task);
        project.AssignTask(task, user);

        Assert.Equal(user.Id, task.AssigneeId);
    }

    [Fact]
    public void AssignTask_ShouldRejectUser_WhenUserBelongsToDifferentTenant()
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

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        project.AddTask(task);

        var act = () => project.AssignTask(task, user);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AssignTask_ShouldRejectTaskNotBelongingToProject()
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

        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        var act = () => project.AssignTask(task, user);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AssignTask_ShouldRejectNullUser()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        project.AddTask(task);

        var act = () => project.AssignTask(task, null!);

        Assert.Throws<ArgumentNullException>(act);
    }
}