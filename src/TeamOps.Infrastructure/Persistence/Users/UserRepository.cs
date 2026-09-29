using Microsoft.EntityFrameworkCore;
using TeamOps.Application.Users;
using TeamOps.Domain.Users;

namespace TeamOps.Infrastructure.Persistence.Users;

public sealed class UserRepository(TeamOpsDbContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        context.Users.Add(user);

        await context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .SingleOrDefaultAsync(x => x.Id == id);
    }
}