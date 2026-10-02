using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Projects;
using TeamOps.Application.Tasks;
using TeamOps.Application.Users;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectController(
    UnassignTaskHandler unassignTaskHandler,
    AssignTaskHandler assignTaskHandler,
    GetProjectHandler getProjectHandler,
    GetProjectsHandler getProjectsHandler,
    CreateProjectHandler createProjectHandler,
    CompleteProjectHandler completeProjectHandler,
    AddProjectMemberHandler addProjectMemberHandler,
    RemoveProjectMemberHandler removeProjectMemberHandler,
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IProjectRepository projectRepository
    ) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command)
    {
        var project = await createProjectHandler.Handle(command);

        return Created($"/api/projects/{project.Id}",
            new
            {
                project.Id,
                project.TenantId,
                project.Name
            });
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var project = await getProjectHandler.Handle(id);

        if (project is null)
            return NotFound();

        return Ok(new
        {
            project.Id,
            project.TenantId,
            project.Name
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects(Guid tenantId)
    {
        var projects = await getProjectsHandler.Handle(tenantId);

        return Ok(projects.Select(project => new
        {
            project.Id,
            project.TenantId,
            project.Name
        }));
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await completeProjectHandler.Handle(new CompleteProjectCommand(id));

        return NoContent();
    }

    [HttpPost("{projectId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> AddMember(Guid projectId, Guid userId)
    {
        var project = await projectRepository.GetByIdAsync(projectId);
        var user = await userRepository.GetByIdAsync(userId);

        if (project is null || user is null)
            return NotFound();

        try
        {
            await addProjectMemberHandler.Handle(new AddProjectMemberCommand(project, user));
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{projectId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid projectId, Guid userId)
    {
        try
        {
            await removeProjectMemberHandler.Handle(new RemoveProjectMemberCommand(projectId, userId));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{projectId:guid}/tasks/{taskId:guid}/assignee/{userId:guid}")]
    public async Task<IActionResult> AssignTask(Guid projectId, Guid taskId, Guid userId)
    {
        var project = await projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return NotFound();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
            return NotFound();

        var task = await taskRepository.GetByIdAsync(taskId);

        if (task is null)
            return NotFound();

        try
        {
            await assignTaskHandler.Handle(new AssignTaskCommand(project, task, user));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        await taskRepository.UpdateAsync(task);

        return NoContent();
    }

    [HttpDelete("{taskId:guid}/assignee")]
    public async Task<IActionResult> Unassign(Guid taskId)
    {
        try
        {
            await unassignTaskHandler.Handle(taskId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}