using TeamOps.Domain.Tasks;

namespace TeamOps.Application.Tasks;

public sealed class CreateTaskHandler
{
    public Task<TaskItem> Handle(CreateTaskCommand command)
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            command.ProjectId,
            command.Title);

        return Task.FromResult(task);
    }
}