using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Security;
using HttpMethods = Microsoft.AspNetCore.Http.HttpMethods;

namespace Tawtheef.Infrastructure.Middlewares
{
    public class RequestSanitizationMiddleware(RequestDelegate next)
    {
        private static readonly Regex HtmlMarkupRegex = new(
            @"<[^>]+>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        private static readonly Regex ObviousDangerousContentRegex = new(
            @"<\s*/?\s*(script|iframe|object|embed)\b" +
            @"|javascript\s*:" +
            @"|vbscript\s*:" +
            @"|data\s*:\s*text/html" +
            @"|on[a-zA-Z]+\s*=",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public async Task Invoke(HttpContext context)
        {
            // Only check POST, PUT, PATCH requests; we ignored GET, DELETE because they should not have a body and are less likely to be used for XSS attacks.
            if (context.Request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Method.Equals(HttpMethods.Put, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Method.Equals(HttpMethods.Patch, StringComparison.OrdinalIgnoreCase))
            {
                var allowRichText = context.GetEndpoint()?
                    .Metadata
                    .GetMetadata<AllowRichTextAttribute>() is not null;

                // 1) Check headers
                if (context.Request.Headers.Any(header => ContainsHtmlMarkup(header.Value.ToString())))
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

                    if (ContainsUnsafeBodyContent(body, allowRichText))
                    {
                        await Reject(context);
                        return;
                    }
                }
                // 3) multipart/form-data
                else if (contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                {
                    var form = await context.Request.ReadFormAsync();

                    if (form.Any(field => ContainsUnsafeBodyContent(field.Value.ToString(), allowRichText)) ||
                        form.Files.Any(file => ContainsHtmlMarkup(file.FileName)))
                    {
                        await Reject(context);
                        return;
                    }
                }
            }

            await next(context);
        }

        private static bool ContainsHtmlMarkup(string input)
            => !string.IsNullOrWhiteSpace(input) && HtmlMarkupRegex.IsMatch(input);

        private static bool ContainsUnsafeBodyContent(string input, bool allowRichText)
            => !string.IsNullOrWhiteSpace(input) &&
               (allowRichText
                   ? ObviousDangerousContentRegex.IsMatch(input)
                   : HtmlMarkupRegex.IsMatch(input));

        private static Task Reject(HttpContext context)
            => context.WriteErrorAsync(
                code: ErrorsCodes.RequestContainsInvalidOrUnsafeContent,
                userMessage: "Request contains invalid or unsafe content.",
                statusCode: StatusCodes.Status400BadRequest,
                blockedBy: "RequestSanitizationMiddleware"
            );
    }
}
