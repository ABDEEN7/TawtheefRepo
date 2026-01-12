using Application.Recruitment.Features.Profile.DTOs;
using FluentResults;

namespace Application.Recruitment.Common.Interfaces.Services.HttpClients;

public interface IMoiClient
{
    Task<IResult<MOEPersonalInfo>> GetPersonalInfoAsync(string qid, DateOnly expiryData, CancellationToken ct = default);
}
