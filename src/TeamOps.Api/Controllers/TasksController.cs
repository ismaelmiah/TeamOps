using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Tasks;
using TeamOps.Domain.Tasks;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(CreateTaskHandler createTaskHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskCommand command)
    {
        TaskItem task;

        try
        {
            task = await createTaskHandler.Handle(command);
        }
        catch
        {
            return BadRequest();
        }

        return Created(
            $"/api/tasks/{task.Id}",
            new
            {
                task.Id,
                task.ProjectId,
                task.Title
            });
    }
}