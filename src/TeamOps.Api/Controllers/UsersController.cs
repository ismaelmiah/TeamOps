using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Users;

namespace TeamOps.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(CreateUserHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var user = await handler.Handle(command);

        return Created(
            $"/api/users/{user.Id}",
            new
            {
                user.Id,
                user.TenantId,
                user.Email,
                user.Name,
                user.Role
            });
    }
}