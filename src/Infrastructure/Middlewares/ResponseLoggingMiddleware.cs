using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace Tawtheef.Infrastructure.Middlewares;

public class ResponseLoggingMiddleware(RequestDelegate next)
{
    // Cap logged body to avoid flooding your sink
    private const int MaxLoggedBytes = 8 * 1024;

    // Only log textual/problem responses
    private static readonly string[] TextualContentTypes =
    [
        "application/json", "text/json",
        "application/problem+json",
        "text/plain", "application/xml", "text/xml"
    ];

    public async Task Invoke(HttpContext ctx)
    {
        var originalBody = ctx.Response.Body;

        await using var buffer = new MemoryStream();
        ctx.Response.Body = buffer;

        try
        {
            await next(ctx); // execute pipeline

            // Snapshot
            buffer.Position = 0;
            var logged = await ReadResponseBodyForLogAsync(ctx.Response, buffer);
            buffer.Position = 0;

            // Enrich context
            using (LogContext.PushProperty("ResponseStatusCode", ctx.Response.StatusCode))
            using (LogContext.PushProperty("ResponseContentType", ctx.Response.ContentType ?? ""))
            using (LogContext.PushProperty("ResponseBody", logged ?? ""))
            using (LogContext.PushProperty("Route", ctx.GetEndpoint()?.DisplayName ?? ""))
            {
                // If it looks like ProblemDetails, extract highlights so search is easy
                if (IsProblemJson(ctx.Response.ContentType) && !string.IsNullOrWhiteSpace(logged))
                {
                    TryExtractProblemHighlights(logged,
                        out var pType, out var title, out var detail, out var errors);

                    using (LogContext.PushProperty("ProblemType", pType ?? ""))
                    using (LogContext.PushProperty("ProblemTitle", title ?? ""))
                    using (LogContext.PushProperty("ProblemDetail", detail ?? ""))
                    using (LogContext.PushProperty("ProblemErrors", errors ?? new object()))
                    {
                        // No explicit Log call needed here if you already log per-request with Serilog RequestLoggingMiddleware.
                        // But if you want an explicit line when non-2xx:
                        if (ctx.Response.StatusCode >= 400)
                            Log.Error("HTTP {Method} {Path} responded {StatusCode}",
                                ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode);
                    }
                }
                else if (ctx.Response.StatusCode >= 400)
                {
                    Log.Error("HTTP {Method} {Path} responded {StatusCode}",
                        ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode);
                }
            }

            // Copy back to response
            await buffer.CopyToAsync(originalBody);
        }
        finally
        {
            ctx.Response.Body = originalBody;
        }
    }

    private static bool IsProblemJson(string? contentType) =>
        !string.IsNullOrWhiteSpace(contentType) &&
        contentType.Contains("application/problem+json", StringComparison.OrdinalIgnoreCase);

    private static bool IsLikelyTextual(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return false;
        var ct = contentType.ToLowerInvariant();

        if (TextualContentTypes.Any(t => ct.Contains(t))) return true;
        return (ct.Contains("multipart/") || ct.Contains("octet-stream") ||
                ct.StartsWith("image/") || ct.StartsWith("audio/") || ct.StartsWith("video/")) && false;
    }

    private static Task<string?> ReadResponseBodyForLogAsync(HttpResponse response, MemoryStream buffer)
    {
        if (!IsLikelyTextual(response.ContentType)) return Task.FromResult<string?>(null);

        byte[] bytes;
        if (buffer.Length > MaxLoggedBytes)
        {
            bytes = buffer.ToArray().AsSpan(0, MaxLoggedBytes).ToArray();
            var text = Encoding.UTF8.GetString(bytes);
            return Task.FromResult(text + " [TRUNCATED]")!;
        }

        bytes = buffer.ToArray();
        return Task.FromResult(Encoding.UTF8.GetString(bytes))!;
    }

    private static void TryExtractProblemHighlights(
        string json,
        out string? type, out string? title, out string? detail, out object? errors)
    {
        type = title = detail = null;
        errors = null;

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("type", out var pType))   type   = pType.GetString();
            if (root.TryGetProperty("title", out var pTitle)) title  = pTitle.GetString();
            if (root.TryGetProperty("detail", out var pDet))  detail = pDet.GetString();

            if (!root.TryGetProperty("errors", out var errs)) return;
            // Convert to a simple dictionary<string, string[]> for logging
            var dict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in errs.EnumerateObject())
            {
                var list = new List<string>();
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    list.AddRange(prop.Value.EnumerateArray().Select(item => item.ToString()));
                }
                else
                {
                    list.Add(prop.Value.ToString());
                }
                dict[prop.Name] = list;
            }
            errors = dict;
        }
        catch
        {
            // ignore parse errors; we still have raw ResponseBody
        }
    }
}
