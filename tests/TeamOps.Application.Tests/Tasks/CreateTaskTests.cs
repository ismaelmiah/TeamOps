using TeamOps.Application.Tasks;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Application.Tests.Tasks;

public class CreateTaskTests
{
    [Fact]
    public async Task Execute_ShouldCreateTask()
    {
        var projectId = Guid.NewGuid();

        var command = new CreateTaskCommand(projectId, "Implement login");

        var handler = new CreateTaskHandler();

        var task = await handler.Handle(command);

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(projectId, task.ProjectId);
        Assert.Equal("Implement login", task.Title);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.Null(task.AssigneeId);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyTitle()
    {
        var command = new CreateTaskCommand(Guid.NewGuid(), "");

        var handler = new CreateTaskHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyProjectId()
    {
        var command = new CreateTaskCommand(Guid.Empty, "Implement login");

        var handler = new CreateTaskHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }
}