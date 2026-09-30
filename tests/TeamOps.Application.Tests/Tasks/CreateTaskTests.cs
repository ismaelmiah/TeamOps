using TeamOps.Application.Tasks;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Application.Tests.Tasks;

public class CreateTaskTests
{
    class FakeTaskRepository : ITaskRepository
    {
        public Task AddAsync(Domain.Tasks.TaskItem task)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Tasks.TaskItem?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public async Task Execute_ShouldCreateTask()
    {
        var projectId = Guid.NewGuid();

        var command = new CreateTaskCommand(projectId, "Implement login");
        var repository = new FakeTaskRepository();
        var handler = new CreateTaskHandler(repository);

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
        var repository = new FakeTaskRepository();
        var handler = new CreateTaskHandler(repository);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyProjectId()
    {
        var command = new CreateTaskCommand(Guid.Empty, "Implement login");
        var repository = new FakeTaskRepository();
        var handler = new CreateTaskHandler(repository);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }
}