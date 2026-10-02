using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public sealed class GetUserHandler(IUserRepository repository)
{
    public async Task<User?> Handle(Guid userId)
    {
        return await repository.GetByIdAsync(userId);
    }
}