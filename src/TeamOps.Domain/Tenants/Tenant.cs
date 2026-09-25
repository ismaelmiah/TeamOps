namespace TeamOps.Domain.Tenants;

public sealed class Tenant
{
    private Tenant(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static Tenant Create(string name)
    {
        return new Tenant(name);
    }
}