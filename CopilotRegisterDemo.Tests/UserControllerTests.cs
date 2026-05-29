using CopilotRegisterDemo.Api.Controllers;
using CopilotRegisterDemo.Api.Models;
using CopilotRegisterDemo.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace CopilotRegisterDemo.Tests;

public class UserControllerTests
{
    [Fact]
    public void Register_Returns_Result_Wrapped_Response()
    {
        var controller = CreateController(new UserService(), "trace-register");
        var request = new RegisterRequest
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "secret123"
        };

        var actionResult = controller.Register(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var result = Assert.IsType<Result<UserDto>>(createdResult.Value);
        Assert.True(result.Succeeded);
        Assert.Equal("trace-register", result.TraceId);
        Assert.Equal("alice", result.Data?.Username);
    }

    [Fact]
    public void GetById_Returns_Result_Wrapped_NotFound_Response()
    {
        var controller = CreateController(new UserService(), "trace-not-found");

        var actionResult = controller.GetById(404);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var result = Assert.IsType<Result<UserDto>>(notFoundResult.Value);
        Assert.False(result.Succeeded);
        Assert.Equal("trace-not-found", result.TraceId);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public void DeleteById_Returns_Result_Wrapped_Success_Response()
    {
        var service = new UserService();
        var registeredUser = service.Register(new RegisterRequest
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "secret123"
        });

        var controller = CreateController(service, "trace-delete");

        var actionResult = controller.DeleteById(registeredUser.Id);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var result = Assert.IsType<Result<bool>>(okResult.Value);
        Assert.True(result.Succeeded);
        Assert.True(result.Data);
        Assert.Equal("trace-delete", result.TraceId);
    }

    [Fact]
    public void DeleteById_Returns_Result_Wrapped_NotFound_Response()
    {
        var controller = CreateController(new UserService(), "trace-delete-not-found");

        var actionResult = controller.DeleteById(404);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var result = Assert.IsType<Result<bool>>(notFoundResult.Value);
        Assert.False(result.Succeeded);
        Assert.Equal("trace-delete-not-found", result.TraceId);
        Assert.Equal("User not found.", result.Message);
    }

    private static UserController CreateController(UserService service, string traceId)
    {
        var controller = new UserController(service, NullLogger<UserController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    TraceIdentifier = traceId
                }
            }
        };

        return controller;
    }
}
