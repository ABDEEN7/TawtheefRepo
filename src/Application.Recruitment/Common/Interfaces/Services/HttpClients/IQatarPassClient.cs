using FluentResults;
using QatarPassEnvelope = Application.Recruitment.Features.Authenticator.DTOs.QatarPassEnvelope;

namespace Application.Recruitment.Common.Interfaces.Services.HttpClients;

public interface IQatarPassClient
{
    Task<IResult<QatarPassEnvelope>> GetDataAsync(string code, CancellationToken ct);
}
