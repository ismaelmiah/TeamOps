namespace TeamOps.Domain.Users;

public sealed class User
{
    private User(Guid id, Guid tenantId, string email, string name)
    {
        Id = id;
        TenantId = tenantId;
        Email = email;
        Name = name;
    }

    public Guid Id { get; }

    public Guid TenantId { get; }

    public string Email { get; private set; }

    public string Name { get; private set; }

    public UserRole Role { get; private set; }

    public static User Create(Guid id, Guid tenantId, string email, string name, UserRole role = UserRole.Member)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(id));

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (name.Length > 100)
            throw new ArgumentException("Name is too long.", nameof(name));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (email.Length > 200)
            throw new ArgumentException("Email is too long.", nameof(email));

        var user = new User(id, tenantId, email.Trim(), name.Trim());
        user.Role = role;

        return user;
    }

    public void Update(string email, string name, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (email.Length > 200)
            throw new ArgumentException("Email cannot exceed 200 characters.", nameof(email));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name is required.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("User name cannot exceed 100 characters.", nameof(name));

        Email = email.Trim();
        Name = name.Trim();
        Role = role;
    }
}