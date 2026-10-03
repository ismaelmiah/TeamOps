using TeamOps.Application.Common;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public sealed class GetUserHandler(
    IUserRepository repository,
    ICurrentTenant currentTenant)
{
    public async Task<User?> Handle(Guid userId)
    {
        return await repository.GetByIdAsync(userId, currentTenant.TenantId);
    }
}