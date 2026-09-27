using Microsoft.EntityFrameworkCore;
using TeamOps.Domain.Tenants;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Infrastructure.Tests.Tenants;

public class TenantPersistenceTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=teamops;Username=postgres;Password=mysecretpassword";

    [Fact]
    public async Task Save_ShouldPersistTenant()
    {
        // Arrange with in-memory database
        var options = new DbContextOptionsBuilder<TeamOpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TeamOpsDbContext(options);

        var tenant = Tenant.Create(
            Guid.NewGuid(),
            "Acme");

        context.Tenants.Add(tenant);

        await context.SaveChangesAsync();

        var savedTenant = await context.Tenants.SingleAsync();

        Assert.Equal(tenant.Id, savedTenant.Id);
        Assert.Equal("Acme", savedTenant.Name);
    }


    [Fact]
    public async Task Save_ShouldPersistTenantToPostgres()
    {
        var options = new DbContextOptionsBuilder<TeamOpsDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new TeamOpsDbContext(options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var tenant = Tenant.Create(
            Guid.NewGuid(),
            "Acme");

        context.Tenants.Add(tenant);

        await context.SaveChangesAsync();

        var savedTenant = await context.Tenants.SingleAsync();

        Assert.Equal(tenant.Id, savedTenant.Id);
        Assert.Equal("Acme", savedTenant.Name);
        Assert.Equal(tenant.CreatedAt, savedTenant.CreatedAt);
    }
}