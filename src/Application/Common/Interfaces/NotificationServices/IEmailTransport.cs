using System.Threading;
using System.Threading.Tasks;
using Tawtheef.Application.Common.Models.Notification;

namespace Tawtheef.Application.Common.Interfaces.NotificationServices;

public interface IEmailTransport
{
    Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default);
}
