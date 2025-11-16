using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;

namespace Tawtheef.Infrastructure.Utils;

/// <summary>
/// Generates a tiny HTML page that sends a postMessage back to the opener window
/// and then automatically closes the popup. Used for OAuth / external login callbacks.
/// </summary>
public static class HtmlPopupCloseScript
{
    /// <summary>
    /// Creates an HTML response that posts a message back to the opener window.
    /// </summary>
    /// <param name="message">The object to post to the opener (will be JSON serialized).</param>
    /// <param name="targetOrigin">The front-end origin (e.g. https://app.example.com).</param>
    /// <returns>A ContentResult containing the HTML + script.</returns>
    public static ContentResult Create(object message, string targetOrigin)
    {
        // Serialize payload safely
        var payload = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        // Escape targetOrigin to avoid double quotes
        var html = $$"""
            <!doctype html>
            <meta charset="utf-8">
            <script>
              (function() {
                try {
                  const payload = {{payload}};
                  const origin = "{{targetOrigin}}";
                  
                  if (window.opener && origin) {
                    window.opener.postMessage(payload, origin);
                  }
                } catch (e) { console.error(e); }
                
                // Close the popup
                // window.close();
              })();
            </script>
            """;

        return new ContentResult
        {
            Content = html,
            ContentType = "text/html"
        };
    }

    /// <summary>
    /// Shortcut for success messages.
    /// </summary>
    public static ContentResult Success(object userData, string targetOrigin)
        => Create(new { type = ExternalLoginMessageTypes.Success, userData }, targetOrigin);

    /// <summary>
    /// Shortcut for error messages.
    /// </summary>
    public static ContentResult Error(object error, string targetOrigin)
        => Create(new { type = ExternalLoginMessageTypes.Error, message = error }, targetOrigin);
}
