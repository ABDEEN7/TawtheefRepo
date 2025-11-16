using Tawtheef.Application.Common.Models.Notification;

namespace Tawtheef.Application.Common.Interfaces.NotificationServices;

public interface IEmailTransport
{
    Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default);
}
