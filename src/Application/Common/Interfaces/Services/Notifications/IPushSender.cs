using Tawtheef.Application.Common.Models.Notification;

namespace Tawtheef.Application.Common.Interfaces.Services.Notifications;

public interface IPushSender
{
    Task<NotificationResponse> SendAsync(
        Guid userId, string title, string body, CancellationToken ct);
}
