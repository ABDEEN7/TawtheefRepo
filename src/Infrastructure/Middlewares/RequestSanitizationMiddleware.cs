using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;
using HttpMethods = Microsoft.AspNetCore.Http.HttpMethods;

namespace Tawtheef.Infrastructure.Middlewares
{
    public class RequestSanitizationMiddleware(RequestDelegate next)
    {
        private static readonly Regex HtmlRegex = new(
            @"<script|</script|<[^>]+>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public async Task Invoke(HttpContext context)
        {
            // Only check POST, PUT, PATCH requests; we ignored GET, DELETE because they should not have a body and are less likely to be used for XSS attacks.
            if (context.Request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Method.Equals(HttpMethods.Put, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Method.Equals(HttpMethods.Patch, StringComparison.OrdinalIgnoreCase))
            {
                // 1) Check headers
                if (context.Request.Headers.Any(header => ContainsHtmlOrScript(header.Value.ToString())))
                {
                    await Reject(context);
                    return;
                }

                var contentType = context.Request.ContentType ?? string.Empty;

                // 2) JSON body
                if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    context.Request.EnableBuffering();

                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (ContainsHtmlOrScript(body))
                    {
                        await Reject(context);
                        return;
                    }
                }
                // 3) multipart/form-data
                else if (contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                {
                    var form = await context.Request.ReadFormAsync();

                    if (form.Any(field => ContainsHtmlOrScript(field.Value.ToString())) || 
                        form.Files.Any(file => ContainsHtmlOrScript(file.FileName)))
                    {
                        await Reject(context);
                        return;
                    }
                }
            }

            await next(context);
        }

        private static bool ContainsHtmlOrScript(string input)
            => !string.IsNullOrWhiteSpace(input) && HtmlRegex.IsMatch(input);

        private static Task Reject(HttpContext context)
            => context.WriteErrorAsync(
                code: ErrorsCodes.RequestContainsInvalidOrUnsafeContent,
                userMessage: "Request contains invalid or unsafe content.",
                statusCode: StatusCodes.Status400BadRequest,
                blockedBy: "RequestSanitizationMiddleware"
            );
    }
}
