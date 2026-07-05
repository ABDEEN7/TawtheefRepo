
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Tawtheef.Application.Common.Exceptions;
using Tawtheef.Infrastructure.Extensions;

namespace Tawtheef.Infrastructure.Middlewares;
public sealed class CustomExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
            return false;

        var correlationId = httpContext.GetCorrelationId();
        var userId = httpContext.GetUserIdOrAnonymous();

        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation error"),
            NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        Log.ForContext("UserId", userId)
           .ForContext("CorrelationId", correlationId)
           .Error(exception, "Exception -> {StatusCode}", statusCode);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path
        };

        // ✅ HANDLE VALIDATION
        if (exception is ValidationException validationEx)
        {
            var groupedErrors = validationEx.Errors
                .GroupBy(e => string.IsNullOrWhiteSpace(e.Field) ? "General" : e.Field)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Message).ToArray()
                );
            problem.Detail = validationEx.Errors[0].Message;
            problem.Extensions["errors"] = groupedErrors;
        }
        else
        {
            problem.Detail = statusCode >= 500
                ? "INTERNAL_SERVER_ERROR"
                : exception.Message;
        }

        problem.Extensions["traceId"] = correlationId;
        problem.Extensions["correlationId"] = correlationId;
        problem.Extensions["ticket"] = correlationId;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.Headers[HttpContextExtensions.CorrelationIdHeaderName] = correlationId;
        httpContext.Response.Headers[HttpContextExtensions.RequestIdHeaderName] = correlationId;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
