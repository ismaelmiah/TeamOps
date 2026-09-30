using Microsoft.EntityFrameworkCore;
using TeamOps.Application.Tasks;
using TeamOps.Domain.Tasks;

namespace TeamOps.Infrastructure.Persistence.Tasks;

public sealed class TaskRepository(
    TeamOpsDbContext context) : ITaskRepository
{
    public async Task AddAsync(TaskItem task)
    {
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await context.Tasks
            .SingleOrDefaultAsync(x => x.Id == id);
    }
}