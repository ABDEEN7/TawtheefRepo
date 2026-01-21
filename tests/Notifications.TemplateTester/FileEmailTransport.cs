using System.Net.Mail;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Notifications.TemplateTester;

internal sealed class FileEmailTransport(EmailSettings emailSettings) : IEmailTransport
{
    public Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(emailSettings.EmailUser),
            Subject = envelope.Subject,
            Body = envelope.HtmlBody,
            IsBodyHtml = true,
        };

        foreach (var to in envelope.To)
            message.To.Add(to);

        foreach (var cc in envelope.Cc ?? [])
            message.CC.Add(cc); // CC FIX

        using var smtp = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort)
        {
            EnableSsl = true,
            // IMPORTANT: no Credentials set (same as your original)
            // If required:
            // Credentials = new NetworkCredential(emailSettings.EmailUser, emailSettings.EmailPass)
        };

        smtp.Send(message);
        return Task.CompletedTask;
    }
}
