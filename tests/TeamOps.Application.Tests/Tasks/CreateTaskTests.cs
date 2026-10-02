using TeamOps.Application.Projects;
using TeamOps.Application.Tasks;
using TeamOps.Application.Tests.Projects;
using TeamOps.Domain.Tasks;
using static TeamOps.Application.Tests.Common.CurrentTenantTests;
using TaskStatus = TeamOps.Domain.Tasks.TaskStatus;

namespace TeamOps.Application.Tests.Tasks;

public class CreateTaskTests
{
    internal class FakeTaskRepository : ITaskRepository
    {
        private List<TaskItem> Tasks { get; } = [];

        public async Task AddAsync(TaskItem task)
        {
            Tasks.Add(task);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            return Tasks.Single(x => x.Id == id);
        }

        public async Task UpdateAsync(TaskItem task)
        {
            var tsk = Tasks.Find(x => x.Id == task.Id);
            if (tsk is not null)
            {
                Tasks.Remove(tsk);
            }
            
            Tasks.Add(task);
        }
    }

    [Fact]
    public async Task Execute_ShouldCreateTask()
    {
        var taskRepository = new FakeTaskRepository();
        var projectRepository = new FakeProjectRepository();
        var handler = new CreateTaskHandler(taskRepository, projectRepository);
        var currentTenant = new TestCurrentTenant(Guid.NewGuid());
        var projectCommand = new CreateProjectCommand("Authentication Service");
        var projectHandler = new CreateProjectHandler(projectRepository, currentTenant);
        var project = await projectHandler.Handle(projectCommand);

        var taskCommand = new CreateTaskCommand(project.Id, "Implement login");
        var task = await handler.Handle(taskCommand);

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(project.Id, task.ProjectId);
        Assert.Equal("Implement login", task.Title);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.Null(task.AssigneeId);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyTitle()
    {
        var taskRepository = new FakeTaskRepository();
        var projectRepository = new FakeProjectRepository();
        var handler = new CreateTaskHandler(taskRepository, projectRepository);
        var currentTenant = new TestCurrentTenant(Guid.NewGuid());
        var projectCommand = new CreateProjectCommand("Authentication Service");
        var projectHandler = new CreateProjectHandler(projectRepository, currentTenant);
        var project = await projectHandler.Handle(projectCommand);

        var taskCommand = new CreateTaskCommand(project.Id, "");

        var act = () => handler.Handle(taskCommand);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task Execute_ShouldRejectEmptyProjectId()
    {
        var command = new CreateTaskCommand(Guid.Empty, "Implement login");
        var taskRepository = new FakeTaskRepository();
        var projectRepository = new FakeProjectRepository();
        var handler = new CreateTaskHandler(taskRepository, projectRepository);

        var act = () => handler.Handle(command);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }
}