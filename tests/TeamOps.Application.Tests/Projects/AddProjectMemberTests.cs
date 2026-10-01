using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Tests.Projects;

public class AddProjectMemberTests
{
    class FakeProjectMemberRepository : IProjectMemberRepository
    {
        private List<ProjectMember> ProjectMembers { get; } = [];
        public async Task AddAsync(ProjectMember member)
        {
            ProjectMembers.Add(member);
        }

        public async Task<ProjectMember?> GetAsync(Guid projectId, Guid userId)
        {
            return await Task.FromResult(ProjectMembers
                .Find(x =>
                    x.ProjectId == projectId &&
                    x.UserId == userId));
        }

        public async Task RemoveAsync(ProjectMember member)
        {
            ProjectMembers.Remove(member);
        }
    }

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
        var repository = new FakeProjectMemberRepository();
        var handler = new AddProjectMemberHandler(repository);

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
        var repository = new FakeProjectMemberRepository();
        var handler = new AddProjectMemberHandler(repository);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}