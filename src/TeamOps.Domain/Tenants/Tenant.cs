namespace TeamOps.Domain.Tenants;

public sealed class Tenant
{
    private Tenant(Guid id, string name, DateTime createdAt)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Name { get; }
    public DateTime CreatedAt { get; }

    public static Tenant Create(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(id));

        if (name is null)
            throw new ArgumentNullException(nameof(name));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name is required.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Tenant name cannot exceed 100 characters.", nameof(name));

        return new Tenant(id, name.Trim(), DateTime.UtcNow);
    }
}