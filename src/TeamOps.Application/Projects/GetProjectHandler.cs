using TeamOps.Application.Common;
using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class GetProjectHandler(IProjectRepository repository, ICurrentTenant currentTenant)
{
    public async Task<Project?> Handle(Guid projectId)
    {
        return await repository.GetByIdAsync(
            projectId,
            currentTenant.TenantId);
    }
}