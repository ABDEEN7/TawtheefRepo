using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Tawtheef.Application.Common.Exceptions;

namespace Tawtheef.Infrastructure.Middlewares;

// ExceptionHandlingMiddleware.cs
public class ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Customize based on an exception type
        var statusCode = exception switch
        {
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception.GetType().Name.Replace("Exception", ""),
            Detail = exception.Message
        };

        // Only include stack trace in development
        if (environment.IsDevelopment())
        {
            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
            problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

// Extension method for easy registration
