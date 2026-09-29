using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class GetProjectHandler(IProjectRepository repository)
{
    public async Task<Project?> Handle(Guid projectId)
    {
        return await repository.GetByIdAsync(projectId);
    }
}