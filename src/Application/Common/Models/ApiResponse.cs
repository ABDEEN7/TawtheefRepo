using FluentResults;

namespace Tawtheef.Application.Common.Models;

/// <summary>
/// Represents a standardized response for API communication, providing success status, data, error messages, and error codes.
/// </summary>
/// <typeparam name="T">The type of data returned by the API response.</typeparam>
public record ApiResponse<T>(bool Success, T? Data, IReadOnlyList<IError>? Error = null)
{
    public static ApiResponse<T> SuccessResponse(T data) => new(true, data);

    public static ApiResponse<T> ErrorResponse(IReadOnlyList<IError> error) 
        => new(false, default, error);
}
