namespace TeamOps.Application.Projects;

public sealed class AddProjectMemberHandler
{
    public Task Handle(AddProjectMemberCommand command)
    {
        command.Project.AddMember(command.User);

        return Task.CompletedTask;
    }
}