using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public interface IProjectMemberRepository
{
    Task AddAsync(ProjectMember member);
}