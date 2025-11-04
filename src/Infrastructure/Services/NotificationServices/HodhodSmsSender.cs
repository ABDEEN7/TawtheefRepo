using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class HodhodSmsSender(ISmsGatewayClient hodhod) : ISmsSender
{
    public async Task<(bool ok, string? providerId, string? error)> SendAsync(string phoneE164, string body, CancellationToken ct)
    {
        var res = await hodhod.SmsPushAsync(phoneE164, body, ct);
        return res.IsSuccess ? (true, res.Value, null) : (false, null, res.Error);
    }
}
