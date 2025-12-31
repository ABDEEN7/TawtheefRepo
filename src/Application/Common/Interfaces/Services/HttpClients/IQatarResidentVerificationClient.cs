using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Application.Common.Interfaces.Services.HttpClients;

public interface IQatarResidentVerificationClient
{
    Task<IResult<QatarResidentVerificationResult>> VerifyAsync(string qid, string phoneNumber, CancellationToken ct = default);
}
