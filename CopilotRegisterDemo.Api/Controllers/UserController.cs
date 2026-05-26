using CopilotRegisterDemo.Api.Models;
using CopilotRegisterDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CopilotRegisterDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public ActionResult<UserDto> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = userService.Register(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UserDto> GetById(int id)
    {
        var user = userService.GetById(id);
        return user is null ? NotFound() : Ok(user);
    }
}
