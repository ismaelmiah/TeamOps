using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;

namespace TeamOps.Application.Tests.Projects;

public class CreateProjectTests
{
    private sealed class FakeProjectRepository : IProjectRepository
    {
        public List<Project> Projects { get; } = [];

        public Task AddAsync(Project project)
        {
            Projects.Add(project);
            return Task.CompletedTask;
        }

        public Task<Project?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Project>> GetByTenantIdAsync(Guid tenantId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Project project)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public async Task Execute_ShouldCreateProject()
    {
        var tenantId = Guid.NewGuid();
        var command = new CreateProjectCommand(tenantId, "Website");
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var project = await handler.Handle(command);

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(tenantId, project.TenantId);
        Assert.Equal("Website", project.Name);
        Assert.Equal(ProjectStatus.Active, project.Status);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyTenantId()
    {
        var command = new CreateProjectCommand(Guid.Empty, "Website");
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyProjectName()
    {
        var command = new CreateProjectCommand(Guid.NewGuid(), "");
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }
}