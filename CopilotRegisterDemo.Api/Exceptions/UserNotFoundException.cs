namespace CopilotRegisterDemo.Api.Exceptions;

/// <summary>
/// Represents a business exception for user-not-found scenarios.
/// </summary>
public sealed class UserNotFoundException : BusinessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The user-not-found error message.</param>
    public UserNotFoundException(string message)
        : base(message)
    {
    }
}
