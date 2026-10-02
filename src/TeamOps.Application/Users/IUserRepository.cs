using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<User>> GetByTenantIdAsync(Guid tenantId);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}