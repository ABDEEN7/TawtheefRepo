using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Application.Common.Interfaces.Services.Notifications;

public interface IEmailSender
{
    //TODO: Use DTO instead of Notification
    Task<NotificationResponse> SendAsync(
        Notification notification, CancellationToken ct);
}
