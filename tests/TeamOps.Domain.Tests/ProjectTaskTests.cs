using TeamOps.Domain.Projects;
using TeamOps.Domain.Tasks;

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
}