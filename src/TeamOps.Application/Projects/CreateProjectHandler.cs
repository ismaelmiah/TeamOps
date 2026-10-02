using TeamOps.Application.Common;
using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class CreateProjectHandler(IProjectRepository repository, ICurrentTenant currentTenant)
{
    public async Task<Project> Handle(CreateProjectCommand command)
    {
        var project = Project.Create(Guid.NewGuid(), currentTenant.TenantId, command.Name);

        await repository.AddAsync(project);

        return project;
    }
}