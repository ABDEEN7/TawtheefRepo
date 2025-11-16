using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Tawtheef.Infrastructure.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-ID";
    private const int MaxLoggedBytes = 4 * 1024;          // cap log size
    private const int BufferThreshold = 64 * 1024;        // 64KB before temp file
    private const int BufferLimit = 4 * 1024 * 1024;      // 4MB max buffered

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

        string? bodyText = null;
        var ctHeader = ctx.Request.ContentType ?? string.Empty;
        var isMultipart = ctHeader.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase);
        var isJson = ctHeader.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);

        // 🔒 Never touch multipart—let the form/file binder own the stream
        if (!HttpMethods.IsGet(ctx.Request.Method) &&
            ctx.Request.ContentLength is > 0 &&
            isJson &&
            !isMultipart)
        {
            // Enable buffering ONCE with limits
            ctx.Request.EnableBuffering(BufferThreshold, BufferLimit);

            // Read only up to MaxLoggedBytes
            ctx.Request.Body.Position = 0;
            using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 16 * 1024, leaveOpen: true);

            char[] charBuf = new char[MaxLoggedBytes];
            int read = await reader.ReadBlockAsync(charBuf, 0, charBuf.Length);
            bodyText = new string(charBuf, 0, read);

            // If body longer than cap, note truncation (don’t try to read the rest)
            if (ctx.Request.ContentLength > MaxLoggedBytes)
                bodyText += " [TRUNCATED]";

            // CRITICAL: rewind so MVC can bind normally
            ctx.Request.Body.Position = 0;
        }

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("RequestBody", bodyText ?? string.Empty))
        {
            ctx.Items["CorrelationId"] = correlationId;
            ctx.Response.Headers[HeaderName] = correlationId;

            await next(ctx);
        }
    }
}
