namespace CopilotRegisterDemo.Api.Exceptions;

/// <summary>
/// Represents a business exception for duplicate user registration attempts.
/// </summary>
public sealed class DuplicateUserException : BusinessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateUserException"/> class.
    /// </summary>
    /// <param name="message">The duplicate user error message.</param>
    public DuplicateUserException(string message)
        : base(message)
    {
    }
}
