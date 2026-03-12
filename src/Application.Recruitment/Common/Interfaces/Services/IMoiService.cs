using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Recruitment.Common.Interfaces.Services;

public interface IMoiService
{
    Task<IResult<MOEPersonalInfo>> GetMoiPersonalInfoWithKawaderCheckAsync(string qid, DateOnly expiryDate,
        CancellationToken cancellationToken);
}
