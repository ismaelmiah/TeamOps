using TeamOps.Domain.Tasks;

namespace TeamOps.Application.Tasks;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);

    Task<TaskItem?> GetByIdAsync(Guid id);
}