using CopilotRegisterDemo.Api.Exceptions;
using CopilotRegisterDemo.Api.Models;
using CopilotRegisterDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CopilotRegisterDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService, ILogger<UserController> logger) : ControllerBase
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>A standardized registration result.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status409Conflict)]
    public ActionResult<Result<UserDto>> Register([FromBody] RegisterRequest request)
    {
        var traceId = HttpContext.TraceIdentifier;
        logger.LogInformation("Registering user {Username}. TraceId: {TraceId}", request.Username, traceId);

        try
        {
            var user = userService.Register(request);
            logger.LogInformation("User {UserId} registered successfully. TraceId: {TraceId}", user.Id, traceId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                Result<UserDto>.Success(user, traceId, "User registered successfully."));
        }
        catch (DuplicateUserException exception)
        {
            logger.LogWarning(exception, "Registration conflict for {Username}. TraceId: {TraceId}", request.Username, traceId);
            return Conflict(Result<UserDto>.Failure(exception.Message, traceId));
        }
    }

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>A standardized user query result.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status404NotFound)]
    public ActionResult<Result<UserDto>> GetById(int id)
    {
        var traceId = HttpContext.TraceIdentifier;
        logger.LogInformation("Getting user {UserId}. TraceId: {TraceId}", id, traceId);

        var user = userService.GetById(id);

        if (user is null)
        {
            logger.LogWarning("User {UserId} was not found. TraceId: {TraceId}", id, traceId);
            return NotFound(Result<UserDto>.Failure("User not found.", traceId));
        }

        logger.LogInformation("User {UserId} retrieved successfully. TraceId: {TraceId}", id, traceId);
        return Ok(Result<UserDto>.Success(user, traceId, "User retrieved successfully."));
    }

    /// <summary>
    /// Deletes a user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>A standardized delete result.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    public ActionResult<Result<bool>> DeleteById(int id)
    {
        var traceId = HttpContext.TraceIdentifier;
        logger.LogInformation("Deleting user {UserId}. TraceId: {TraceId}", id, traceId);

        try
        {
            userService.DeleteById(id);
            logger.LogInformation("User {UserId} deleted successfully. TraceId: {TraceId}", id, traceId);
            return Ok(Result<bool>.Success(true, traceId, "User deleted successfully."));
        }
        catch (UserNotFoundException exception)
        {
            logger.LogWarning(exception, "Delete failed for user {UserId}. TraceId: {TraceId}", id, traceId);
            return NotFound(Result<bool>.Failure(exception.Message, traceId));
        }
    }
}
