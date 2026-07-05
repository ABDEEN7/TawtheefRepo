using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Tawtheef.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public const string CorrelationIdItemKey = "CorrelationId";
    public const string CorrelationIdHeaderName = "X-Correlation-ID";
    public const string RequestIdHeaderName = "X-Request-ID";

    public static string? GetClientIpAddress(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded;
        return context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    public static string GetCorrelationId(this HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationIdItemKey, out var value) &&
            value is not null &&
            !string.IsNullOrWhiteSpace(value.ToString()))
        {
            return value.ToString()!;
        }

        return context.TraceIdentifier;
    }

    public static string GetUserIdOrAnonymous(this HttpContext context)
    {
        return context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "anonymous";
    }
}
