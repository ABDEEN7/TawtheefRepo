using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class HodhodSmsSender(ISmsGatewayClient hodhod) : ISmsSender
{
    public async Task<NotificationResponse> SendAsync(string phoneE164, string body, CancellationToken ct)
    {
        var res = await hodhod.SmsPushAsync(phoneE164, body, ct);
        return res.IsSuccess ? new (true, res.Value, null) : new (false, null, res.Errors);
    }
}
