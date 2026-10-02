using TeamOps.Application.Common;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public sealed class CreateUserHandler(
    IUserRepository repository,
    ICurrentTenant currentTenant)
{
    public async Task<User> Handle(CreateUserCommand command)
    {
        var user = User.Create(
            Guid.NewGuid(),
            currentTenant.TenantId,
            command.Email,
            command.Name,
            command.Role);

        await repository.AddAsync(user);

        return user;
    }
}