using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class AddProjectMemberHandler(
    IProjectMemberRepository repository)
{
    public async Task Handle(AddProjectMemberCommand command)
    {
        command.Project.AddMember(command.User);

        var member = ProjectMember.Create(
                    Guid.NewGuid(),
                    command.Project.Id,
                    command.User.Id);

        await repository.AddAsync(member);
    }
}