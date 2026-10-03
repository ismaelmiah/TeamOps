using TeamOps.Application.Common;

namespace TeamOps.Application.Projects;

public sealed class DeleteProjectHandler(IProjectRepository repository, ICurrentTenant currentTenant)
{
    public async Task Handle(Guid projectId)
    {
        // var project = await repository.GetByIdAsync(projectId);
        var project = await repository.GetByIdAsync(projectId, currentTenant.TenantId);

        if (project is null)
            throw new KeyNotFoundException("Project not found.");

        await repository.DeleteAsync(project);
    }
}