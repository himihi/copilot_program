using System.ComponentModel.DataAnnotations;
using CopilotRegisterDemo.Api.Models;

namespace CopilotRegisterDemo.Api.Services;

public class UserService
{
    private readonly List<RegisteredUser> _users = [];
    private readonly object _lock = new();
    private int _nextId = 1;

    public UserDto Register(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRequest(request);

        lock (_lock)
        {
            // Check if a user with the same email or username already exists
            if (_users.Any(user => string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            if (_users.Any(user => string.Equals(user.Username, request.Username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A user with this username already exists.");
            }

            var user = new RegisteredUser(
                _nextId++,
                request.Username.Trim(),
                request.Email.Trim(),
                request.Password);

            _users.Add(user);

            return MapToDto(user);
        }
    }

    public async  Task<UserDto> RegisterAsync(RegisterRequest request)
    {        
        return await Task.Run(() => Register(request));
    }

    public UserDto? GetById(int id)
    {
        lock (_lock)
        {
            var user = _users.SingleOrDefault(item => item.Id == id);

            return user is null ? null : MapToDto(user);
        }
    }

    private static UserDto MapToDto(RegisteredUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password
        };
    }

    private static void ValidateRequest(RegisterRequest request)
    {
        var validationContext = new ValidationContext(request);
        Validator.ValidateObject(request, validationContext, validateAllProperties: true);
    }

    private sealed record RegisteredUser(int Id, string Username, string Email, string Password);
}
