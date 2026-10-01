namespace TeamOps.Application.Tasks;

public sealed class HoldTaskHandler(ITaskRepository repository)
{
    public async Task Handle(Guid taskId)
    {
        var task = await repository.GetByIdAsync(taskId);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        task.Hold();

        await repository.UpdateAsync(task);
    }
}