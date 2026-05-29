namespace CopilotRegisterDemo.Api.Models;

/// <summary>
/// Represents a standard API response envelope.
/// </summary>
/// <typeparam name="T">The response payload type.</typeparam>
public sealed class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the request succeeded.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the response payload.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets the request trace identifier.
    /// </summary>
    public string TraceId { get; init; } = string.Empty;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="data">The response payload.</param>
    /// <param name="traceId">The current request trace identifier.</param>
    /// <param name="message">The success message.</param>
    /// <returns>A successful <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Success(T data, string traceId, string message = "Success")
    {
        return new Result<T>
        {
            Succeeded = true,
            Message = message,
            Data = data,
            TraceId = traceId
        };
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="traceId">The current request trace identifier.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Failure(string message, string traceId)
    {
        return new Result<T>
        {
            Succeeded = false,
            Message = message,
            Data = default,
            TraceId = traceId
        };
    }
}
