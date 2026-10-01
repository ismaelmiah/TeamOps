using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;

namespace TeamOps.Application.Tests.Projects;

internal sealed class FakeProjectRepository : IProjectRepository
{
    public List<Project> Projects { get; } = [];

    public Task AddAsync(Project project)
    {
        Projects.Add(project);
        return Task.CompletedTask;
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(Projects.Find(x => x.Id == id));
    }

    public async Task<IReadOnlyList<Project>> GetByTenantIdAsync(Guid tenantId)
    {
        return await Task.FromResult(Projects
            .Where(x => x.TenantId == tenantId).ToList());
    }

    public async Task UpdateAsync(Project project)
    {
        var p = Projects.Single(pro => pro.Id == project.Id);

        Projects.Remove(p);
        Projects.Add(project);
    }
}