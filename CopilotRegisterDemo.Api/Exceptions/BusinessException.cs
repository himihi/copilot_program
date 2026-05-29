namespace CopilotRegisterDemo.Api.Exceptions;

/// <summary>
/// Represents a business exception that can be safely surfaced to callers.
/// </summary>
public abstract class BusinessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessException"/> class.
    /// </summary>
    /// <param name="message">The business error message.</param>
    protected BusinessException(string message)
        : base(message)
    {
    }
}
