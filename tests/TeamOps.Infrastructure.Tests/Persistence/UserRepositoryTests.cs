using Microsoft.EntityFrameworkCore;
using TeamOps.Domain.Users;
using TeamOps.Infrastructure.Persistence;
using TeamOps.Infrastructure.Persistence.Users;

namespace TeamOps.Infrastructure.Tests.Persistence;

public class UserRepositoryTests
{
    private string connectionString = "Host=localhost;Port=5432;Database=teamops;Username=postgres;Password=mysecretpassword";

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        var options = new DbContextOptionsBuilder<TeamOpsDbContext>().UseNpgsql(connectionString).Options;

        await using var context = new TeamOpsDbContext(options);

        await context.Database.MigrateAsync();

        var repository = new UserRepository(context);

        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            $"user-{Guid.NewGuid()}@example.com",
            "John Smith");

        await repository.AddAsync(user);

        var savedUser = await context.Users
            .SingleAsync(x => x.Id == user.Id);

        Assert.Equal(user.Id, savedUser.Id);
        Assert.Equal(user.TenantId, savedUser.TenantId);
        Assert.Equal(user.Email, savedUser.Email);
        Assert.Equal(user.Name, savedUser.Name);
        Assert.Equal(user.Role, savedUser.Role);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        var options = new DbContextOptionsBuilder<TeamOpsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new TeamOpsDbContext(options);

        await context.Database.MigrateAsync();

        var tenantId = Guid.NewGuid();
        var user = User.Create(
            Guid.NewGuid(),
            tenantId,
            $"user-{Guid.NewGuid()}@example.com",
            "Jane Smith");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetByIdAsync(user.Id, tenantId);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.TenantId, result.TenantId);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<TeamOpsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new TeamOpsDbContext(options);

        await context.Database.MigrateAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(result);
    }
}