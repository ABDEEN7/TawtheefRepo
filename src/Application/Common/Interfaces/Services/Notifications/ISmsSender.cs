using Tawtheef.Application.Common.Models.Notification;

namespace Tawtheef.Application.Common.Interfaces.Services.Notifications;

public interface ISmsSender
{
    Task<NotificationResponse> SendAsync(string phoneE164, string body, CancellationToken ct);
}
