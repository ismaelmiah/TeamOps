using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace TeamOps.Infrastructure.Persistence.Projects;

public sealed class ProjectMemberRepository(TeamOpsDbContext context) : IProjectMemberRepository
{
    public async Task AddAsync(ProjectMember member)
    {
        context.ProjectMembers.Add(member);
        await context.SaveChangesAsync();
    }

    public async Task<ProjectMember?> GetAsync(Guid projectId, Guid userId)
    {
        return await context.ProjectMembers
            .SingleOrDefaultAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == userId);
    }

    public async Task RemoveAsync(ProjectMember member)
    {
        context.ProjectMembers.Remove(member);
        await context.SaveChangesAsync();
    }
}