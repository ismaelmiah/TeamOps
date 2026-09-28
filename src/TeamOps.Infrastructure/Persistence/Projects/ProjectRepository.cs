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
}