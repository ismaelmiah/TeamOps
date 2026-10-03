using TeamOps.Application.Common;
using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class GetProjectsHandler(IProjectRepository repository, ICurrentTenant currentTenant)
{
    public async Task<IReadOnlyList<Project>> Handle()
    {
        return await repository.GetByTenantIdAsync(currentTenant.TenantId);
    }
}