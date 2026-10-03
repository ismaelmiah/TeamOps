using TeamOps.Application.Common;

namespace TeamOps.Application.Projects;

public sealed class CompleteProjectHandler(
    IProjectRepository repository,
    ICurrentTenant currentTenant)
{
    public async Task Handle(CompleteProjectCommand command)
    {
        var project = await repository.GetByIdAsync(command.ProjectId, currentTenant.TenantId);

        if (project is null)
            throw new KeyNotFoundException(
                "Project was not found.");

        project.Complete();

        await repository.UpdateAsync(project);
    }
}