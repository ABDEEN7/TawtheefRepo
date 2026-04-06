using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Application.Common.Interfaces.Services.HttpClients;

public interface IMoiClient
{
    Task<IResult<MOEPersonalInfo>> GetPersonalInfoAsync(string qid, DateOnly expiryData, CancellationToken ct = default);
}
