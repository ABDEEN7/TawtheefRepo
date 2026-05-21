using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class DurableEmailSender(
    IEmailTransport transport, 
    IEmailTemplateRenderer renderer) : IEmailSender
{
    public async Task<NotificationResponse> SendAsync(Notification notification, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(notification.ToAddress))
            return NotificationResponse.Failure("TO_ADDRESS_REQUIRED");

        var body = notification.Body;
        var plainText = notification.PlainTextBody;

        // If body is missing but template key exists, render it on the fly
        if (string.IsNullOrWhiteSpace(body) && !string.IsNullOrWhiteSpace(notification.TemplateKey))
        {
            try
            {
                body = await renderer.RenderHtmlAsync(notification.TemplateKey, notification.PayloadJson ?? "{}", notification.Language);
                plainText = await renderer.RenderTextAsync(notification.TemplateKey, notification.PayloadJson ?? "{}", notification.Language);
            }
            catch (Exception ex)
            {
                return NotificationResponse.Failure($"TEMPLATE_RENDER_ERROR: {ex.Message}");
            }
        }

        var to = notification.ToAddress
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var cc = (notification.CcAddress ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var envelope = new EmailEnvelope(
            to,
            cc,
            notification.Subject ?? "Outreach, Recruitment, and Manpower Planning System",
            body ?? string.Empty,
            plainText
        );

        try
        {
            await transport.SendAsync(envelope, ct);
            return NotificationResponse.Success(notification.Id.ToString());
        }
        catch (Exception ex)
        {
            return NotificationResponse.Failure(ex.Message);
        }
    }
}
