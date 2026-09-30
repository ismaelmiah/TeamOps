using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public interface IProjectMemberRepository
{
    Task AddAsync(ProjectMember member);

    Task<ProjectMember?> GetAsync(Guid projectId, Guid userId);

    Task RemoveAsync(ProjectMember member);
}