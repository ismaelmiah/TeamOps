using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public sealed class CreateUserHandler(IUserRepository repository)
{
    public async Task<User> Handle(CreateUserCommand command)
    {
        var user = User.Create(
            Guid.NewGuid(),
            command.TenantId,
            command.Email,
            command.Name,
            command.Role);

        await repository.AddAsync(user);

        return user;
    }
}