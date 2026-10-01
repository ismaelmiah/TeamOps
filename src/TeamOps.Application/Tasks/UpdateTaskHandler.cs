using TeamOps.Domain.Tasks;

namespace TeamOps.Application.Tasks;

public sealed class UpdateTaskHandler(
    ITaskRepository repository)
{
    public async Task Handle(UpdateTaskCommand command)
    {
        var task = await repository.GetByIdAsync(command.TaskId);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        task.UpdateTitle(command.Title);

        await repository.UpdateAsync(task);
    }
}