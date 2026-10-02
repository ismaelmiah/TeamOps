namespace TeamOps.Application.Projects;

public sealed class DeleteProjectHandler(IProjectRepository repository)
{
    public async Task Handle(Guid projectId)
    {
        var project = await repository.GetByIdAsync(projectId);

        if (project is null)
            throw new KeyNotFoundException("Project not found.");

        await repository.DeleteAsync(project);
    }
}