using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class CreateProjectHandler
{
    public Task<Project> Handle(CreateProjectCommand command)
    {
        var project = Project.Create(Guid.NewGuid(), command.TenantId, command.Name);

        return Task.FromResult(project);
    }
}