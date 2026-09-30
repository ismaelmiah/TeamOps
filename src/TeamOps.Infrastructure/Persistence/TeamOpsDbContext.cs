using Microsoft.EntityFrameworkCore;
using TeamOps.Domain.Projects;
using TeamOps.Domain.Tasks;
using TeamOps.Domain.Tenants;
using TeamOps.Domain.Users;
using TeamOps.Infrastructure.Persistence.Configurations;

namespace TeamOps.Infrastructure.Persistence;

public sealed class TeamOpsDbContext(DbContextOptions<TeamOpsDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
    }
}