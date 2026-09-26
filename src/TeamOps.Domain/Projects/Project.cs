using TeamOps.Domain.Users;

namespace TeamOps.Domain.Projects;

public sealed class Project
{
    private readonly HashSet<Guid> _memberIds = [];

    private Project(Guid id, Guid tenantId, string name, ProjectStatus status)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Status = status;
    }

    public Guid Id { get; }

    public Guid TenantId { get; }

    public string Name { get; }
    public ProjectStatus Status { get; private set; }
    public IReadOnlyCollection<Guid> MemberIds => _memberIds;

    public static Project Create(Guid id, Guid tenantId, string name)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name is required.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Project name cannot exceed 100 characters.", nameof(name));

        return new Project(id, tenantId, name.Trim(), ProjectStatus.Active);
    }

    public void Complete()
    {
        if (Status == ProjectStatus.Completed)
            throw new InvalidOperationException("Project is already completed.");

        Status = ProjectStatus.Completed;
    }

    public void AddMember(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (user.Id == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(user.Id));

        if (user.TenantId != TenantId)
            throw new InvalidOperationException("User does not belong to the same tenant as the project.");

        if (!_memberIds.Add(user.Id))
            throw new InvalidOperationException("User is already a project member.");
    }

    public void RemoveMember(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (!_memberIds.Remove(userId))
            throw new InvalidOperationException("User is not a project member.");
    }
}