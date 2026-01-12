using FluentResults;

namespace Application.Recruitment.Common.Interfaces.Services.HttpClients;

public interface IQatarResidentVerificationClient
{
    Task<IResult<bool>> VerifyAsync(string qid, string phoneNumber, CancellationToken ct = default);
}
