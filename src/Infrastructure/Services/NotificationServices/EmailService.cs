using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations;
using Tawtheef.Infrastructure.Templates.EmailConfirmation;
using Tawtheef.Infrastructure.Templates.EmailVerificationSuccess;
using Tawtheef.Infrastructure.Templates.ResetPassword;

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
    private async Task EnqueueUsingHtml(
        string html, string subject,
        List<string> to, List<string>? cc = null,
        CancellationToken ct = default)
    {
        var plainText = HtmlAgilityPackRegex().Replace(html, string.Empty);
        var env = new EmailEnvelope(to, cc, subject, html, plainText);
        await emailQueue.EnqueueAsync(env, ct);
    }
    
    public Task SendConfirmationLinkEmailAsync(string email, string? recipientName, string link)
        => EnqueueUsingTemplate("EmailConfirmation", "Please Confirm Your Email Address",
            [email], new EmailConfirmationModel(link,recipientName));

    public Task SendEmailVerificationEmailSuccessAsync(string email, string userName)
        => EnqueueUsingTemplate("EmailVerificationSuccess", "✅ Email Verification Successful",
            [email], new EmailVerificationSuccessModel(userName));

    public Task SendResetPassword(string email, string link)
        => EnqueueUsingTemplate("ResetPassword", "🔐 Reset Your Password",
            [email], new ResetPasswordModel(link));
    
    public Task SendTemplateAsync(
        string templateKey, string subject, List<string> to, object model,
        List<string>? cc = null, CancellationToken ct = default)
        => EnqueueUsingTemplate(templateKey, subject, to, model, cc, ct);

    public Task SendHtmlAsync(
        string subject, List<string> to, string html,
        List<string>? cc = null, CancellationToken ct = default)
        => EnqueueUsingHtml(html, subject, to, cc, ct);
    
    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlAgilityPackRegex();
}
