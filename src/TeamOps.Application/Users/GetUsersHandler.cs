using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public sealed class GetUsersHandler(IUserRepository repository)
{
    public async Task<IReadOnlyList<User>> Handle(Guid tenantId)
    {
        return await repository.GetByTenantIdAsync(tenantId);
    }
}