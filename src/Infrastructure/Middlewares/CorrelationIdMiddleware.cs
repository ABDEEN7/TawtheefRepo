using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Tawtheef.Infrastructure.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-ID";
    public async Task Invoke(HttpContext ctx)
    {
        // Skip noisy endpoints
        if (ctx.Request.Path.StartsWithSegments("/health"))
        {
            await next(ctx);
            return;
        }

        var correlationId =
            (ctx.Request.Headers.TryGetValue(HeaderName, out var h) && !string.IsNullOrWhiteSpace(h))
                ? h.ToString()
                : ctx.TraceIdentifier;

        var userId = ctx.User.FindFirst("sub")?.Value
                  ?? ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? "anonymous";

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("UserId", userId))
        {
            ctx.Items["CorrelationId"] = correlationId;
            ctx.Response.Headers[HeaderName] = correlationId;

            await next(ctx);
        }
    }
}
