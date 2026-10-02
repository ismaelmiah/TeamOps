using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;
using static TeamOps.Application.Tests.Common.CurrentTenantTests;

namespace TeamOps.Application.Tests.Projects;

public partial class CreateProjectTests
{

    [Fact]
    public async Task Execute_ShouldCreateProject()
    {
        var tenantId = Guid.NewGuid();
        var currentTenant = new TestCurrentTenant(tenantId);
        var command = new CreateProjectCommand("Website");
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository, currentTenant);

        var project = await handler.Handle(command);

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(tenantId, project.TenantId);
        Assert.Equal("Website", project.Name);
        Assert.Equal(ProjectStatus.Active, project.Status);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyTenantId()
    {
        var currentTenant = new TestCurrentTenant(Guid.Empty);
        var command = new CreateProjectCommand("Website");
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository, currentTenant);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyProjectName()
    {
        var currentTenant = new TestCurrentTenant(Guid.NewGuid());
        var command = new CreateProjectCommand("");
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository, currentTenant);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task Handle_uses_current_tenant()
    {
        var tenantId = Guid.NewGuid();

        var repository = new FakeProjectRepository();

        var currentTenant = new TestCurrentTenant(tenantId);

        var handler = new CreateProjectHandler(
            repository,
            currentTenant);

        var command = new CreateProjectCommand("Test Project");

        var project = await handler.Handle(command);

        Assert.Equal(tenantId, project.TenantId);
    }
}