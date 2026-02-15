using System.Net.Mail;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Notifications.TemplateTester;

internal sealed class FileEmailTransport(EmailSettings emailSettings) : IEmailTransport
{
    public Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default)
    {
        using var message = new MailMessage();
        message.From = new MailAddress(emailSettings.EmailUser);
        message.Subject = envelope.Subject;
        message.Body = envelope.HtmlBody;
        message.IsBodyHtml = true;

        foreach (var to in envelope.To)
            message.To.Add(to);

        foreach (var cc in envelope.Cc ?? [])
            message.CC.Add(cc); // CC FIX

        using var smtp = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort);
        smtp.EnableSsl = true;

        smtp.Send(message);
        return Task.CompletedTask;
    }
}
