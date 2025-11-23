using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services.HttpClients;


public interface ISmsGatewayClient
{
    Task<IResult<string>> SmsPushAsync(string mobile, string message, CancellationToken ct);
}
