using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Projects;
using TeamOps.Application.Users;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectController(
    CreateProjectHandler createProjectHandler,
    GetProjectHandler getProjectHandler,
    GetProjectsHandler getProjectsHandler,
    CompleteProjectHandler completeProjectHandler,
    IUserRepository userRepository,
    AddProjectMemberHandler addProjectMemberHandler,
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
}