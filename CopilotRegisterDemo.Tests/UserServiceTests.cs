using System.ComponentModel.DataAnnotations;
using CopilotRegisterDemo.Api.Exceptions;
using CopilotRegisterDemo.Api.Models;
using CopilotRegisterDemo.Api.Services;

namespace CopilotRegisterDemo.Tests;

public class UserServiceTests
{
    [Fact]
    public void Register_Returns_UserDto_When_Request_Is_Valid()
    {
        var service = new UserService();
        var request = new RegisterRequest
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "secret123"
        };

        var result = service.Register(request);

        Assert.Equal(1, result.Id);
        Assert.Equal("alice", result.Username);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Equal("secret123", result.Password);
    }

    [Fact]
    public void GetById_Returns_UserDto_With_Password()
    {
        var service = new UserService();
        var registeredUser = service.Register(new RegisterRequest
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "secret123"
        });

        var result = service.GetById(registeredUser.Id);

        Assert.NotNull(result);
        Assert.Equal(registeredUser.Id, result.Id);
        Assert.Equal("secret123", result.Password);
    }

    [Fact]
    public void Register_Throws_When_Email_Already_Exists()
    {
        var service = new UserService();
        service.Register(new RegisterRequest
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "secret123"
        });

        var action = () => service.Register(new RegisterRequest
        {
            Username = "alice2",
            Email = "ALICE@example.com",
            Password = "secret456"
        });

        var exception = Assert.Throws<DuplicateUserException>(action);

        Assert.Equal("A user with this email already exists.", exception.Message);
    }

    [Fact]
    public void Register_Throws_When_Request_Fails_Validation()
    {
        var service = new UserService();

        var action = () => service.Register(new RegisterRequest
        {
            Username = "ab",
            Email = "not-an-email",
            Password = "123"
        });

        Assert.Throws<ValidationException>(action);
    }

    [Fact]
    public void DeleteById_Removes_User_When_User_Exists()
    {
        var service = new UserService();
        var registeredUser = service.Register(new RegisterRequest
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "secret123"
        });

        service.DeleteById(registeredUser.Id);

        var result = service.GetById(registeredUser.Id);
        Assert.Null(result);
    }

    [Fact]
    public void DeleteById_Throws_When_User_Does_Not_Exist()
    {
        var service = new UserService();

        var action = () => service.DeleteById(404);

        var exception = Assert.Throws<UserNotFoundException>(action);
        Assert.Equal("User not found.", exception.Message);
    }
}
