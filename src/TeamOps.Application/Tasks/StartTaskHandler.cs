namespace TeamOps.Application.Tasks;

public sealed class StartTaskHandler(
    ITaskRepository repository)
{
    public async Task Handle(StartTaskCommand command)
    {
        var task = await repository.GetByIdAsync(command.TaskId);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        task.Start();

        await repository.UpdateAsync(task);
    }
}