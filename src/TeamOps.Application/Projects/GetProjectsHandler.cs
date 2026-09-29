using TeamOps.Domain.Projects;

namespace TeamOps.Application.Projects;

public sealed class GetProjectsHandler(IProjectRepository repository)
{
    public async Task<IReadOnlyList<Project>> Handle(Guid tenantId)
    {
        return await repository.GetByTenantIdAsync(tenantId);
    }
}