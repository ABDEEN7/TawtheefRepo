using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class DurableEmailSender(IEmailTransport transport) : IEmailSender
{
    public async Task<NotificationResponse> SendAsync(Notification notification, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(notification.ToAddress))
            return NotificationResponse.Failure("TO_ADDRESS_REQUIRED");

        var to = notification.ToAddress
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var cc = (notification.CcAddress ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var envelope = new EmailEnvelope(
            to,
            cc,
            notification.Subject ?? "Tawtheef",
            notification.Body,
            notification.PlainTextBody
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
