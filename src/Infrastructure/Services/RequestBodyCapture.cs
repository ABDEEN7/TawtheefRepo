using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Logges;

namespace Tawtheef.Infrastructure.Services;

public sealed class RequestBodyCapture(IHostEnvironment env, IOptions<RequestBodyLoggingOptions> opt)
    : IRequestBodyCapture
{
    private readonly RequestBodyLoggingOptions _opt = opt.Value;

    public async Task<string?> TryGetRedactedBodyAsync(HttpContext ctx)
    {
        if (!ShouldCapture(ctx)) return null;

        var ct = ctx.Request.ContentType ?? string.Empty;
        var isJson = ct.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        if (!isJson) return null;

        // Avoid touching multipart/form-data, files, etc.
        if (ct.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            return null;

        if (HttpMethods.IsGet(ctx.Request.Method) || HttpMethods.IsHead(ctx.Request.Method))
            return null;

        if (ctx.Request.ContentLength is null or <= 0)
            return null;

        // Enable buffering with limits (once)
        ctx.Request.EnableBuffering(_opt.BufferThreshold, _opt.BufferLimit);

        // Read up to MaxLoggedBytes (bytes, not chars)
        ctx.Request.Body.Position = 0;

        using var ms = new MemoryStream(capacity: Math.Min((int)ctx.Request.ContentLength.Value, _opt.MaxLoggedBytes));
        var buffer = new byte[16 * 1024];
        int remaining = _opt.MaxLoggedBytes;

        while (remaining > 0)
        {
            int read = await ctx.Request.Body.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, remaining)));
            if (read <= 0) break;

            await ms.WriteAsync(buffer.AsMemory(0, read));
            remaining -= read;
        }

        // rewind for MVC binding
        ctx.Request.Body.Position = 0;

        var raw = Encoding.UTF8.GetString(ms.ToArray());
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var truncated = (ctx.Request.ContentLength.Value > _opt.MaxLoggedBytes);

        // Redact structurally (best effort)
        string redacted;
        try
        {
            redacted = JsonRedactor.RedactJson(raw, _opt.SensitiveKeys);
        }
        catch
        {
            // If JSON parsing fails, do not log raw content
            return "[UNPARSEABLE_JSON]";
        }

        if (truncated) redacted += " [TRUNCATED]";
        return redacted;
    }

    private bool ShouldCapture(HttpContext ctx)
    {
        if (!_opt.Enabled) return false;

        if (_opt.Mode == RequestBodyLoggingMode.None) return false;

        if (_opt.Mode == RequestBodyLoggingMode.DevelopmentOnly && !env.IsDevelopment())
            return false;

        // Health always excluded
        if (ctx.Request.Path.StartsWithSegments("/health"))
            return false;

        // Denylist wins
        foreach (var p in _opt.DenyPaths)
        {
            if (!string.IsNullOrWhiteSpace(p) && ctx.Request.Path.StartsWithSegments(p))
                return false;
        }

        if (_opt.Mode == RequestBodyLoggingMode.AllowlistedEndpoints)
        {
            foreach (var p in _opt.AllowPaths)
            {
                if (!string.IsNullOrWhiteSpace(p) && ctx.Request.Path.StartsWithSegments(p))
                    return true;
            }
            return false; // not allowlisted
        }

        // DevelopmentOnly and passed checks
        return true;
    }
}
