using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;
using TeamOps.Domain.Tasks;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Tests.Projects;

public class AssignTaskTests
{
    [Fact]
    public async Task Execute_ShouldAssignTaskToUser()
    {
        var tenantId = Guid.NewGuid();

        var project = Project.Create(
            Guid.NewGuid(),
            tenantId,
            "Website");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        var user = User.Create(
            Guid.NewGuid(),
            tenantId,
            "user@example.com",
            "John");

        project.AddTask(task);

        var command = new AssignTaskCommand(project, task, user);

        var handler = new AssignTaskHandler();

        await handler.Handle(command);

        Assert.Equal(user.Id, task.AssigneeId);
    }

    [Fact]
    public async Task Execute_ShouldRejectUserFromDifferentTenant()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            project.Id,
            "Implement login");

        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user@example.com",
            "John");

        project.AddTask(task);

        var command = new AssignTaskCommand(project, task, user);

        var handler = new AssignTaskHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectTaskNotBelongingToProject()
    {
        var tenantId = Guid.NewGuid();

        var project = Project.Create(
            Guid.NewGuid(),
            tenantId,
            "Website");

        var otherProject = Project.Create(
            Guid.NewGuid(),
            tenantId,
            "Mobile App");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            otherProject.Id,
            "Implement login");

        var user = User.Create(
            Guid.NewGuid(),
            tenantId,
            "user@example.com",
            "John");

        var command = new AssignTaskCommand(project, task, user);

        var handler = new AssignTaskHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}