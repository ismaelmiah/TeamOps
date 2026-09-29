using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public interface IProjectRepository
{
    Task AddAsync(Project project);

    Task<Project?> GetByIdAsync(Guid id);
    Task UpdateAsync(Project project);
}