using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Tests.Projects;

public class AddProjectMemberTests
{
    [Fact]
    public async Task Execute_ShouldAddMemberToProject()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var user = User.Create(
            Guid.NewGuid(),
            project.TenantId,
            "user@example.com",
            "John");

        var command = new AddProjectMemberCommand(project, user);

        var handler = new AddProjectMemberHandler();

        await handler.Handle(command);

        Assert.Contains(user.Id, project.MemberIds);
    }

    [Fact]
    public async Task Execute_ShouldRejectUserFromDifferentTenant()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Website");

        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user@example.com",
            "John");

        var command = new AddProjectMemberCommand(project, user);

        var handler = new AddProjectMemberHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}