namespace TeamOps.Domain.Projects;

public sealed class ProjectMember
{
    private ProjectMember(Guid id, Guid projectId, Guid userId)
    {
        Id = id;
        ProjectId = projectId;
        UserId = userId;
    }

    public Guid Id { get; }
    public Guid ProjectId { get; }
    public Guid UserId { get; }

    public static ProjectMember Create(Guid id, Guid projectId, Guid userId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Project member ID is required.", nameof(id));

        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID is required.", nameof(projectId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        return new ProjectMember(id, projectId, userId);
    }
}