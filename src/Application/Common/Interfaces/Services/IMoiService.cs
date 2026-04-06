using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IMoiService
{
    Task<IResult<MOEPersonalInfo>> GetMoiPersonalInfoWithKawaderCheckAsync(string qid, DateOnly expiryDate,
        CancellationToken cancellationToken);
}
