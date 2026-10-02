using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Users;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(
    CreateUserHandler createUserHandler,
    GetUsersHandler getUsersHandler,
    GetUserHandler getUserHandler,
    UpdateUserHandler updateUserHandler,
    DeleteUserHandler deleteUserHandler
    ) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> Get(Guid userId)
    {
        var user = await getUserHandler.Handle(userId);

        if (user is null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.TenantId,
            user.Email,
            user.Name,
            user.Role
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] Guid tenantId)
    {
        var users = await getUsersHandler.Handle(tenantId);

        return Ok(users.Select(user => new
        {
            user.Id,
            user.TenantId,
            user.Email,
            user.Name,
            user.Role
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var user = await createUserHandler.Handle(command);

        return Created($"/api/users/{user.Id}",
            new
            {
                user.Id,
                user.TenantId,
                user.Email,
                user.Name,
                user.Role
            });
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(Guid userId, UpdateUserCommand command)
    {
        try
        {
            await updateUserHandler.Handle(userId, command);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(Guid userId)
    {
        try
        {
            await deleteUserHandler.Handle(userId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}