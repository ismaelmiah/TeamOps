using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task DeleteAsync(Project project);
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByIdAsync(Guid id, Guid tenantId);
    Task<IReadOnlyList<Project>> GetByTenantIdAsync(Guid tenantId);
    Task UpdateAsync(Project project);
    
}