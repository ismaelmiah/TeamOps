namespace TeamOps.Application.Projects;

public sealed class RemoveProjectMemberHandler(IProjectMemberRepository repository)
{
    public async Task Handle(RemoveProjectMemberCommand command)
    {
        var member = await repository.GetAsync(command.ProjectId, command.UserId);

        if (member is null)
            throw new InvalidOperationException("User is not a project member.");

        await repository.RemoveAsync(member);
    }
}