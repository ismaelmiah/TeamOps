using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Projects;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectController(CreateProjectHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command)
    {
        var project = await handler.Handle(command);

        return Created($"/api/projects/{project.Id}",
            new
            {
                project.Id,
                project.TenantId,
                project.Name
            });
    }
}