using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Exceptions;

namespace Tawtheef.Infrastructure.Middlewares;

public sealed class CustomExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Correlation & user context
        var correlationId = httpContext.TraceIdentifier;
        var userId = httpContext.User.FindFirst("sub")?.Value
                     ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? "anonymous";

        // Map to status/title
        var (statusCode, title) = exception switch
        {
            ValidationException      => (StatusCodes.Status400BadRequest, "Validation error"),
            NotFoundException        => (StatusCodes.Status404NotFound, "Not found"),
            ConflictException        => (StatusCodes.Status409Conflict, "Conflict"),
            UnauthorizedException    => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ForbiddenAccessException       => (StatusCodes.Status403Forbidden, "Forbidden"),
            _                        => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        // Log once with rich context (Seq will index these properties)
        // Log.ForContext("UserId", userId)
        //    .ForContext("CorrelationId", correlationId)
        //    .ForContext("Ticket", correlationId)
        //    .ForContext("Path", httpContext.Request.Path)
        //    .ForContext("Method", httpContext.Request.Method)
        //    .Error(exception, "Unhandled exception -> {StatusCode}", statusCode);

        // Build RFC 7807 payload
        var problem = new ProblemDetails
        {
            Status   = statusCode,
            Title    = title,
            Instance = httpContext.Request.Path,
            Type     = statusCode >= 500 ? "https://httpstatuses.com/500" : null,
            // For 5xx: generic message only (no internal details leaked)
            Detail = statusCode >= 500 ? $"Something went wrong. Please contact support with ticket number: {correlationId}." :
                // For 4xx: show an exception message (or customize per exception)
                exception.Message,
            Extensions =
            {
                // Add useful metadata extensions (visible to the client)
                ["ticket"] = correlationId,
                ["traceId"] = correlationId,
                ["userId"] = userId
            }
        };

        // If you have a ValidationException with an error dictionary, attach it
        if (exception is ValidationException { Errors: { } errors })
        {
            problem.Extensions["errors"] = errors; // adapt to your ValidationException shape
        }

        // Echo headers so callers can display or log them client-side
        httpContext.Response.Headers["X-Correlation-ID"] = correlationId;
        httpContext.Response.Headers["X-Ticket-Number"]  = correlationId;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true; // we handled it
    }
}
