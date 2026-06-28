using Microsoft.AspNetCore.Http;
using Serilog.Context;
using Tawtheef.Infrastructure.Extensions;

namespace Tawtheef.Infrastructure.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const int MaxCorrelationIdLength = 128;

    public async Task Invoke(HttpContext ctx)
    {
        // Skip noisy endpoints
        if (ctx.Request.Path.StartsWithSegments("/health"))
        {
            await next(ctx);
            return;
        }

        var correlationId = ResolveCorrelationId(ctx);
        ctx.TraceIdentifier = correlationId;
        ctx.Items[HttpContextExtensions.CorrelationIdItemKey] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestId", correlationId))
        using (LogContext.PushProperty("UserId", ctx.GetUserIdOrAnonymous()))
        {
            ctx.Response.Headers[HttpContextExtensions.CorrelationIdHeaderName] = correlationId;
            ctx.Response.Headers[HttpContextExtensions.RequestIdHeaderName] = correlationId;

            await next(ctx);
        }
    }

    private static string ResolveCorrelationId(HttpContext ctx)
    {
        return TryGetValidHeader(ctx, HttpContextExtensions.CorrelationIdHeaderName, out var correlationId)
            ? correlationId
            : TryGetValidHeader(ctx, HttpContextExtensions.RequestIdHeaderName, out var requestId)
                ? requestId
                : ctx.TraceIdentifier;
    }

    private static bool TryGetValidHeader(HttpContext ctx, string headerName, out string value)
    {
        value = string.Empty;

        if (!ctx.Request.Headers.TryGetValue(headerName, out var headerValue))
            return false;

        var candidate = headerValue.ToString().Trim();
        if (string.IsNullOrWhiteSpace(candidate) || candidate.Length > MaxCorrelationIdLength)
            return false;

        if (candidate.Any(ch => char.IsControl(ch) || char.IsWhiteSpace(ch)))
            return false;

        value = candidate;
        return true;
    }
}
