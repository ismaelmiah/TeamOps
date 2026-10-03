using TeamOps.Application.Common;

namespace TeamOps.Application.Users;

public sealed class DeleteUserHandler(
    IUserRepository repository,
    ICurrentTenant currentTenant)
{
    public async Task Handle(Guid userId)
    {
        var user = await repository.GetByIdAsync(userId, currentTenant.TenantId);

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        await repository.DeleteAsync(user);
    }
}