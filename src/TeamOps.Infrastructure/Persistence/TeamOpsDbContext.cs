using Microsoft.EntityFrameworkCore;
using TeamOps.Domain.Tenants;
using TeamOps.Infrastructure.Persistence.Configurations;

namespace TeamOps.Infrastructure.Persistence;

public sealed class TeamOpsDbContext(DbContextOptions<TeamOpsDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
    }
}