using Microsoft.EntityFrameworkCore;
using TeamOps.Domain.Users;
using TeamOps.Infrastructure.Persistence;

namespace TeamOps.Infrastructure.Tests.Persistence;

public class UserPersistenceTests
{
    private string connectionString = "Host=localhost;Port=5432;Database=teamops;Username=postgres;Password=mysecretpassword";

    [Fact]
    public async Task User_ShouldBePersisted()
    {
        var options = new DbContextOptionsBuilder<TeamOpsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new TeamOpsDbContext(options);

        await context.Database.MigrateAsync();

        var user = User.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "john@example.com",
            "John Smith");

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var savedUser = await context.Users
            .SingleAsync(x => x.Id == user.Id);

        Assert.Equal(user.Id, savedUser.Id);
        Assert.Equal(user.TenantId, savedUser.TenantId);
        Assert.Equal("john@example.com", savedUser.Email);
        Assert.Equal("John Smith", savedUser.Name);
        Assert.Equal(UserRole.Member, savedUser.Role);
    }
}