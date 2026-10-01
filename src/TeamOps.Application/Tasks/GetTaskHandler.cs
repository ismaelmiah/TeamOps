using TeamOps.Domain.Tasks;

namespace TeamOps.Application.Tasks;

public sealed class GetTaskHandler(ITaskRepository repository)
{
    public async Task<TaskItem?> Handle(Guid taskId)
    {
        return await repository.GetByIdAsync(taskId);
    }
}