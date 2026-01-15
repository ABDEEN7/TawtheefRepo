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
        var modelType = model.GetType();
        var sendMethod = typeof(IEmailService).GetMethod(nameof(IEmailService.SendTemplateAsync));
        if (sendMethod is null)
            throw new InvalidOperationException("SendTemplateAsync method not found on IEmailService.");

        var genericMethod = sendMethod.MakeGenericMethod(modelType);
        var task = (Task?)genericMethod.Invoke(
            emailService,
            [notification.TemplateKey, notification.Subject ?? "Tawtheef", to, model, cc, ct]);
        if (task is null)
            throw new InvalidOperationException("Failed to invoke SendTemplateAsync.");

        await task;

        return NotificationResponse.Success(notification.Id.ToString());
    }
}
