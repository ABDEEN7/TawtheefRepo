using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services.HttpClients;

public interface IQatarResidentVerificationClient
{
    Task<IResult<bool>> VerifyAsync(string qid, string phoneNumber, CancellationToken ct = default);
}
