using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Infrastructure.Extensions;

public static class ResultExtensions
{
    extension<T>(Result<T> result)
    {
        public IActionResult ToActionResult()
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(ApiResponse<T>.SuccessResponse(result.Value));
            }

            return HandleErrorResult(result.Error);
        }
    }

    extension(Result result)
    {
        public IActionResult ToActionResult()
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(ApiResponse<object?>.SuccessResponse(null));
            }

            return HandleErrorResult(result.Error);
        }
    }

    private static IActionResult HandleErrorResult(string error)
    {
        var apiResponse = ApiResponse<object>.ErrorResponse(error, error);

        return error switch
        {
            // { Code: "NotFound" } => new NotFoundObjectResult(apiResponse),
            // { Code: "Validation" } => new BadRequestObjectResult(apiResponse),
            // { Code: "Conflict" } => new ConflictObjectResult(apiResponse),
            // { Code: "Unauthorized" } => new UnauthorizedObjectResult(apiResponse),
            // { Code: "Forbidden" } => new ObjectResult(apiResponse) { StatusCode = 403 },
            _ => new ObjectResult(apiResponse) { StatusCode = 500 }
        };
    }
}
