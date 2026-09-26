using TeamOps.Domain.Projects;

namespace TeamOps.Domain.Tests;

public class ProjectMemberTests
{
    [Fact]
    public void Create_ShouldCreateMember_WhenValidDataProvided()
    {
        var id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var member = ProjectMember.Create(
            id,
            projectId,
            userId);

        Assert.Equal(id, member.Id);
        Assert.Equal(projectId, member.ProjectId);
        Assert.Equal(userId, member.UserId);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldThrow()
    {
        var act = () => ProjectMember.Create(
            Guid.Empty,
            Guid.NewGuid(),
            Guid.NewGuid());

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmptyProjectId_ShouldThrow()
    {
        var act = () => ProjectMember.Create(
            Guid.NewGuid(),
            Guid.Empty,
            Guid.NewGuid());

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrow()
    {
        var act = () => ProjectMember.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }
}