namespace Tawtheef.Application.Common.Models;

/// <summary>
/// Represents a standardized response for API communication, providing success status, data, error messages, and error codes.
/// </summary>
/// <typeparam name="T">The type of data returned by the API response.</typeparam>
public record ApiResponse<T>(bool Success, T? Data, string? Error = null, string? ErrorCode = null)
{
    public static ApiResponse<T> SuccessResponse(T data) => new(true, data);

    public static ApiResponse<T> ErrorResponse(string error, string? errorCode = null) 
        => new(false, default, error, errorCode);
}
