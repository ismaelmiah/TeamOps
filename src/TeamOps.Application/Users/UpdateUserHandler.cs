using TeamOps.Application.Common;

namespace TeamOps.Application.Users;

public sealed class UpdateUserHandler(
    IUserRepository repository,
    ICurrentTenant currentTenant)
{
    public async Task Handle(Guid userId, UpdateUserCommand command)
    {
        var user = await repository.GetByIdAsync(userId, currentTenant.TenantId);

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        user.Update(
            command.Email,
            command.Name,
            command.Role);

        await repository.UpdateAsync(user);
    }
}