using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Auth.Commands;
using MyFinance.Application.Auth.Queries;
using MyFinance.Application.Auth.Dto;

namespace MyFinance.WebApi.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        return await Mediator.Send(new GetUsersQuery());
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await Mediator.Send(new DeleteUserCommand(id));
        
        if (!result)
            return NotFound("User not found.");
            
        return NoContent();
    }
}
