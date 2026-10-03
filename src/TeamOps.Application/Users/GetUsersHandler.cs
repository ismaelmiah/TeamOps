using TeamOps.Application.Common;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public sealed class GetUsersHandler(
    IUserRepository repository,
    ICurrentTenant currentTenant)
{
    public async Task<IReadOnlyList<User>> Handle()
    {
        return await repository.GetByTenantIdAsync(currentTenant.TenantId);
    }
}