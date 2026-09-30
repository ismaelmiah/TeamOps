using TeamOps.Domain.Tasks;

namespace TeamOps.Application.Tasks;

public sealed class CreateTaskHandler(ITaskRepository repository)
{
    public async Task<TaskItem> Handle(CreateTaskCommand command)
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            command.ProjectId,
            command.Title);

        await repository.AddAsync(task);

        return task;
    }
}