using Microsoft.AspNetCore.Http;

namespace Tawtheef.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static string? GetClientIpAddress(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded;
        return context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }
}
