using TeamOps.Application.Projects;
using TeamOps.Application.Tasks;
using TeamOps.Application.Tests.Projects;
using static TeamOps.Application.Tests.Common.CurrentTenantTests;
using static TeamOps.Application.Tests.Tasks.CreateTaskTests;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Application.Tests.Tasks;

public class CompleteTaskTests
{
    [Fact]
    public async Task Execute_ShouldCompleteInProgressTask()
    {
        var taskRepository = new FakeTaskRepository();
        var projectRepository = new FakeProjectRepository();
        var currentTenant = new TestCurrentTenant(Guid.NewGuid());
        var projectCommand = new CreateProjectCommand("Authentication Service");
        var projectHandler = new CreateProjectHandler(projectRepository, currentTenant);
        var project = await projectHandler.Handle(projectCommand);

        var createHandler = new CreateTaskHandler(taskRepository, projectRepository, currentTenant);
        var taskCommand = new CreateTaskCommand(project.Id, "Implement login");
        var task = await createHandler.Handle(taskCommand);

        task.Start();

        var completeHandler = new CompleteTaskHandler(taskRepository);

        await completeHandler.Handle(task.Id);

        Assert.Equal(TaskStatus.Completed, task.Status);
    }

    [Fact]
    public async Task Execute_ShouldRejectPendingTask()
    {
        var taskRepository = new FakeTaskRepository();
        var projectRepository = new FakeProjectRepository();
        var currentTenant = new TestCurrentTenant(Guid.NewGuid());
        var projectCommand = new CreateProjectCommand("Authentication Service");
        var projectHandler = new CreateProjectHandler(projectRepository, currentTenant);
        var project = await projectHandler.Handle(projectCommand);

        var createHandler = new CreateTaskHandler(taskRepository, projectRepository, currentTenant);
        var taskCommand = new CreateTaskCommand(project.Id, "Implement login");
        var task = await createHandler.Handle(taskCommand);

        var completeHandler = new CompleteTaskHandler(taskRepository);

        var act = () => completeHandler.Handle(task.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectAlreadyCompletedTask()
    {
        var taskRepository = new FakeTaskRepository();
        var projectRepository = new FakeProjectRepository();
        var currentTenant = new TestCurrentTenant(Guid.NewGuid());
        var projectCommand = new CreateProjectCommand("Authentication Service");
        var projectHandler = new CreateProjectHandler(projectRepository, currentTenant);
        var project = await projectHandler.Handle(projectCommand);
        var createHandler = new CreateTaskHandler(taskRepository, projectRepository, currentTenant);
        var taskCommand = new CreateTaskCommand(project.Id, "Implement login");
        var task = await createHandler.Handle(taskCommand);

        task.Start();

        var completeHandler = new CompleteTaskHandler(taskRepository);
        var command = new CompleteTaskCommand(task);

        await completeHandler.Handle(task.Id);

        var act = () => completeHandler.Handle(task.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}