using TeamOps.Application.Common;
using TeamOps.Application.Projects;
using TeamOps.Domain.Tasks;

namespace TeamOps.Application.Tasks;

public sealed class CreateTaskHandler(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    ICurrentTenant currentTenant)
{
    public async Task<TaskItem> Handle(CreateTaskCommand command)
    {
        var project = await projectRepository.GetByIdAsync(command.ProjectId, currentTenant.TenantId);

        if (project is null)
            throw new ArgumentException("Project not found.");

        if (string.IsNullOrWhiteSpace(command.Title))
            throw new ArgumentException("Task title is required");

        var task = TaskItem.Create(
            Guid.NewGuid(),
            command.ProjectId,
            command.Title);

        project.AddTask(task);

        await taskRepository.AddAsync(task);

        return task;
    }
}