namespace TeamOps.Application.Projects;

public sealed class AssignTaskHandler
{
    public Task Handle(AssignTaskCommand command)
    {
        command.Project.AssignTask(command.Task, command.User);

        return Task.CompletedTask;
    }
}