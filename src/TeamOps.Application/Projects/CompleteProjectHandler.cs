namespace TeamOps.Application.Projects;

public sealed class CompleteProjectHandler(IProjectRepository repository)
{
    public async Task Handle(CompleteProjectCommand command)
    {
        var project = await repository.GetByIdAsync(
            command.ProjectId);

        if (project is null)
            throw new KeyNotFoundException(
                "Project was not found.");

        project.Complete();

        await repository.UpdateAsync(project);
    }
}