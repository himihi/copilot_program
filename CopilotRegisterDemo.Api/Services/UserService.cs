using System.ComponentModel.DataAnnotations;
using CopilotRegisterDemo.Api.Exceptions;
using CopilotRegisterDemo.Api.Models;

namespace CopilotRegisterDemo.Api.Services;

public class UserService
{
    private readonly List<RegisteredUser> _users = [];
    private readonly object _lock = new();
    private int _nextId = 1;

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>The registered user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="ValidationException">Thrown when the request is invalid.</exception>
    /// <exception cref="DuplicateUserException">Thrown when the username or email already exists.</exception>
    public UserDto Register(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRequest(request);

        var normalizedUsername = request.Username.Trim();
        var normalizedEmail = request.Email.Trim();

        lock (_lock)
        {
            if (_users.Any(user => string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateUserException("A user with this email already exists.");
            }

            if (_users.Any(user => string.Equals(user.Username, normalizedUsername, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateUserException("A user with this username already exists.");
            }

            var user = new RegisteredUser(
                _nextId++,
                normalizedUsername,
                normalizedEmail,
                request.Password);

            _users.Add(user);

            return MapToDto(user);
        }
    }

    /// <summary>
    /// Registers a new user asynchronously.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>The registered user.</returns>
    public Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        return Task.FromResult(Register(request));
    }

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>The matched user when found; otherwise <see langword="null"/>.</returns>
    public UserDto? GetById(int id)
    {
        lock (_lock)
        {
            var user = _users.SingleOrDefault(item => item.Id == id);

            return user is null ? null : MapToDto(user);
        }
    }

    /// <summary>
    /// Deletes a user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <exception cref="UserNotFoundException">Thrown when the user does not exist.</exception>
    public void DeleteById(int id)
    {
        lock (_lock)
        {
            var user = _users.SingleOrDefault(item => item.Id == id);

            if (user is null)
            {
                throw new UserNotFoundException("User not found.");
            }

            _users.Remove(user);
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
