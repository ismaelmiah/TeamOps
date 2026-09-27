using TeamOps.Application.Tasks;
using TeamOps.Domain.Tasks;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Application.Tests.Tasks;

public class CompleteTaskTests
{
    [Fact]
    public async Task Execute_ShouldCompleteInProgressTask()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        task.Start();

        var command = new CompleteTaskCommand(task);

        var handler = new CompleteTaskHandler();

        await handler.Handle(command);

        Assert.Equal(TaskStatus.Completed, task.Status);
    }

    [Fact]
    public async Task Execute_ShouldRejectPendingTask()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        var command = new CompleteTaskCommand(task);

        var handler = new CompleteTaskHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectAlreadyCompletedTask()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Implement login");

        task.Start();
        task.Complete();

        var command = new CompleteTaskCommand(task);

        var handler = new CompleteTaskHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}