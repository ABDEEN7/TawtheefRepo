using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using HttpMethods = Microsoft.AspNetCore.Http.HttpMethods;

namespace Tawtheef.Infrastructure.Middlewares
{
    public class RequestSanitizationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private static readonly Regex HtmlRegex = new(
            @"<script|</script|<[^>]+>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
        private const string RejectionMessage = "Request not accepted.";

        public async Task Invoke(HttpContext context)
        {
            // Only check POST requests
            if (context.Request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase))
            {
                // Check headers
                foreach (var header in context.Request.Headers)
                {
                    if (ContainsHtmlOrScript(header.Value.ToString()))
                    {
                        await WriteRejectedResponseAsync(context);
                        return;
                    }
                }

                var contentType = context.Request.ContentType ?? string.Empty;

                // POST: JSON body
                if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    context.Request.EnableBuffering();

                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (ContainsHtmlOrScript(body))
                    {
                        await WriteRejectedResponseAsync(context);
                        return;
                    }
                }

                // POST: multipart/form-data (FormData)
                if (contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                {
                    var form = await context.Request.ReadFormAsync();

                    // Check form fields
                    foreach (var field in form)
                    {
                        var value = field.Value.ToString();
                        if (ContainsHtmlOrScript(value))
                        {
                            await WriteRejectedResponseAsync(context);
                            return;
                        }
                    }

                    // Check file names
                    foreach (var file in form.Files)
                    {
                        if (ContainsHtmlOrScript(file.FileName))
                        {
                            await WriteRejectedResponseAsync(context);
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }

        private static bool ContainsHtmlOrScript(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return HtmlRegex.IsMatch(input);
        }


        private static async Task WriteRejectedResponseAsync(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Headers["X-Blocked-By"] = "RequestSanitizationMiddleware";

            var errorObj = new { message = RejectionMessage, statusCode = context.Response.StatusCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorObj), Encoding.UTF8);

        }
    }
}
