using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class NullPushSender : IPushSender
{
    public Task<NotificationResponse> SendAsync(Guid userId, string title, string body, CancellationToken ct)
        => Task.FromResult(NotificationResponse.Failure("PUSH_NOT_CONFIGURED"));
}
