using CSharpFunctionalExtensions;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Application.Common.Interfaces.Services.HttpClients;

public interface IQatarPassClient
{
    Task<Result<QatarPassEnvelope>> GetDataAsync(string code, CancellationToken ct);
}
