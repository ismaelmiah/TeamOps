using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;

namespace TeamOps.Infrastructure.Persistence.Projects;

public sealed class ProjectMemberRepository(TeamOpsDbContext context) : IProjectMemberRepository
{
    public async Task AddAsync(ProjectMember member)
    {
        context.ProjectMembers.Add(member);
        await context.SaveChangesAsync();
    }
}