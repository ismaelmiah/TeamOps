using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class CreateProjectHandler(IProjectRepository repository)
{
    public async Task<Project> Handle(CreateProjectCommand command)
    {
        var project = Project.Create(Guid.NewGuid(), command.TenantId, command.Name);

        await repository.AddAsync(project);

        return project;
    }
}