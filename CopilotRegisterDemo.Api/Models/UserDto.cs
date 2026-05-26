namespace CopilotRegisterDemo.Api.Models;

public class UserDto
{
    public int Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;
    
    public string Password { get; init; } = string.Empty;
}
