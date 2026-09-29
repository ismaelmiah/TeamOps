using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Projects;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectController(CreateProjectHandler createProjectHandler, GetProjectHandler getProjectHandler) : ControllerBase
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
}