using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Infrastructure.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this IResult<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(ApiResponse<T>.SuccessResponse(result.Value));
        }

        return HandleErrorResult(result.Errors);
    }

    private static IActionResult HandleErrorResult(IReadOnlyList<IError> errors)
    {
        var first = errors.FirstOrDefault();

        var apiResponse = ApiResponse<object?>.ErrorResponse(errors);

        if (first != null &&
            first.Metadata.TryGetValue("StatusCode", out var statusObj) &&
            statusObj is int statusFromMeta)
        {
            return new ObjectResult(apiResponse)
            {
                StatusCode = statusFromMeta
            };
        }

        var code = first != null
            ? (first.Metadata.TryGetValue("Code", out var codeObj) ? codeObj as string : null) 
              ?? first.Message
            : null;

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
}
