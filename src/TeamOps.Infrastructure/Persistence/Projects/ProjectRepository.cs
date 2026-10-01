using Microsoft.EntityFrameworkCore;
using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;

namespace TeamOps.Infrastructure.Persistence.Projects;

public sealed class ProjectRepository(TeamOpsDbContext context) : IProjectRepository
{
    public async Task AddAsync(Project project)
    {
        context.Projects.Add(project);

        await context.SaveChangesAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await context.Projects.SingleOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<IReadOnlyList<Project>> GetByTenantIdAsync(Guid tenantId)
    {
        return await context.Projects
            .Where(x => x.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        context.Projects.Update(project);

        await context.SaveChangesAsync();
    }
}