using FluentResults;

namespace Tawtheef.Application.Common.Models;

/// <summary>
/// Represents a standardized response for API communication, providing success status, data, error messages, and error codes.
/// </summary>
/// <typeparam name="T">The type of data returned by the API response.</typeparam>
public record ApiResponse<T>(
    bool Success,
    T? Data,
    IReadOnlyList<IError>? Error = null,
    string? Message = null,
    string? TraceId = null)
{
    public static ApiResponse<T> SuccessResponse(T data, string? message = null, string? traceId = null)
        => new(true, data, null, message, traceId);

    public static ApiResponse<T> ErrorResponse(IReadOnlyList<IError> error, string? message = null, string? traceId = null)
        => new(false, default, error, message, traceId);
}
