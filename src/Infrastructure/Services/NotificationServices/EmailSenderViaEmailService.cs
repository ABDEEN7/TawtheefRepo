using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Infrastructure.Utils;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class EmailSenderViaEmailService(IEmailService emailService) : IEmailSender
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

        // Body is JSON → model depends on TemplateKey
        var model = NotificationBodyDeserializer.DeserializeBody(notification.TemplateKey, notification.PayloadJson);
        await emailService.SendTemplateAsync(
            notification.TemplateKey, notification.Subject ?? "Tawtheef",
            to, model, cc, ct);

        return NotificationResponse.Success(notification.Id.ToString());
    }
}
