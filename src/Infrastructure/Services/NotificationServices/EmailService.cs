using System.Text.RegularExpressions;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public partial class EmailService(
    IEmailQueue emailQueue,
    IEmailTemplateRenderer renderer) : IEmailService
{
    private async Task EnqueueUsingTemplate<T>(
        string templateKey, string subject,
        List<string> to, T model, List<string>? cc = null,
        CancellationToken ct = default)
    {
            var html = await renderer.RenderHtmlAsync(templateKey, model);
            var text = await renderer.RenderTextAsync(templateKey, model);
            var plainText = string.IsNullOrWhiteSpace(text) ? HtmlAgilityPackRegex().Replace(html, string.Empty) : text;
            var env = new EmailEnvelope(to, cc, subject, html, plainText);
            await emailQueue.EnqueueAsync(env, ct);
    }
    
    public Task SendTemplateAsync<T>(
        string templateKey, string subject, List<string> to, T model,
        List<string>? cc = null, CancellationToken ct = default)
        => EnqueueUsingTemplate(templateKey, subject, to, model, cc, ct);

    
    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlAgilityPackRegex();
}
