using System.Diagnostics;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Models;
using Error = FluentResults.Error;

namespace Tawtheef.Infrastructure.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this IResult<T> result)
    {
        var traceId = GetTraceId();
        var includeDetails = ShouldIncludeDetailedErrors();

        if (result.IsSuccess)
        {
            return new OkObjectResult(ApiResponse<T>.SuccessResponse(result.Value, traceId: traceId));
        }

        return HandleErrorResult(result.Errors, traceId, includeDetails);
    }

    private static IActionResult HandleErrorResult(IReadOnlyList<IError> errors, string traceId, bool includeDetails)
    {
        var first = errors.FirstOrDefault();
        var code = ExtractCode(first);
        var userMessage = BuildUserMessage(first, code);

        var safeErrors = includeDetails
            ? errors
            : SanitizeErrors(errors);

        var apiResponse = ApiResponse<object?>.ErrorResponse(safeErrors, userMessage, traceId);

        if (first != null &&
            first.Metadata.TryGetValue("StatusCode", out var statusObj) &&
            statusObj is int statusFromMeta)
        {
            return new ObjectResult(apiResponse)
            {
                StatusCode = statusFromMeta
            };
        }

        return code switch
        {
            "NotFound"      => new NotFoundObjectResult(apiResponse),
            "Validation"    => new BadRequestObjectResult(apiResponse),
            "Conflict"      => new ConflictObjectResult(apiResponse),
            "Unauthorized"  => new UnauthorizedObjectResult(apiResponse),
            "Forbidden"     => new ObjectResult(apiResponse) { StatusCode = StatusCodes.Status403Forbidden },

            _ => new ObjectResult(apiResponse) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }

    private static string? ExtractCode(IError? error)
    {
        if (error == null)
        {
            return null;
        }

        return (error.Metadata.TryGetValue("Code", out var codeObj) ? codeObj as string : null)
              ?? error.Message;
    }

    private static string BuildUserMessage(IError? error, string? code)
    {
        if (error?.Metadata.TryGetValue("UserMessage", out var userMessageObj) == true &&
            userMessageObj is string explicitMessage &&
            !string.IsNullOrWhiteSpace(explicitMessage))
        {
            return explicitMessage;
        }

        return code switch
        {
            "Validation"    => "Please review the highlighted fields and try again.",
            "Unauthorized"  => "Your session expired. Please sign in and try again.",
            "Forbidden"     => "You do not have permission to perform this action.",
            "NotFound"      => "We could not find what you are looking for.",
            "Conflict"      => "This action conflicts with existing data.",
            _               => "Something went wrong. Please try again or contact support."
        };
    }

    private static string GetTraceId()
    {
        return Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }

    private static IReadOnlyList<IError> SanitizeErrors(IReadOnlyList<IError> errors)
    {
        if (errors.Count == 0)
        {
            return errors;
        }

        return errors.Select(error =>
        {
            var safeError = new Error(error.Message);

            foreach (var metadata in error.Metadata)
            {
                if (IsSafeMetadata(metadata.Key))
                {
                    safeError.WithMetadata(metadata.Key, metadata.Value);
                }
            }

            return (IError)safeError;
        }).ToArray();
    }

    private static bool IsSafeMetadata(string key)
    {
        return key.Equals("Code", StringComparison.OrdinalIgnoreCase)
            || key.Equals("UserMessage", StringComparison.OrdinalIgnoreCase)
            || key.Equals("StatusCode", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldIncludeDetailedErrors()
    {
        var aspnetcoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var dotnetEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(aspnetcoreEnv, "Development", StringComparison.OrdinalIgnoreCase)
            || string.Equals(dotnetEnv, "Development", StringComparison.OrdinalIgnoreCase);
    }
}
