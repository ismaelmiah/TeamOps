using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Tasks;
using TeamOps.Domain.Tasks;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(
    CreateTaskHandler createTaskHandler,
    StartTaskHandler startTaskHandler,
    HoldTaskHandler holdTaskHandler,
    CompleteTaskHandler completeTaskHandler) : ControllerBase
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

    [HttpPost("{taskId:guid}/start")]
    public async Task<IActionResult> Start(Guid taskId)
    {
        try
        {
            await startTaskHandler.Handle(new StartTaskCommand(taskId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpPost("{taskId:guid}/hold")]
    public async Task<IActionResult> Hold(Guid taskId)
    {
        try
        {
            await holdTaskHandler.Handle(taskId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpPost("{taskId:guid}/complete")]
    public async Task<IActionResult> Complete(Guid taskId)
    {
        try
        {
            await completeTaskHandler.Handle(taskId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }

        return NoContent();
    }
}