using TeamOps.Application.Projects;
using TeamOps.Domain.Projects;

namespace TeamOps.Application.Tests.Projects;

public class CreateProjectTests
{
    [Fact]
    public async Task Execute_ShouldCreateProject()
    {
        var tenantId = Guid.NewGuid();
        var command = new CreateProjectCommand(tenantId, "Website");
        var handler = new CreateProjectHandler();

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
        var handler = new CreateProjectHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyProjectName()
    {
        var command = new CreateProjectCommand(Guid.NewGuid(), "");
        var handler = new CreateProjectHandler();

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }
}