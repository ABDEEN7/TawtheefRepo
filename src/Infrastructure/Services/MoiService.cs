using Application.Recruitment.Common.Interfaces.Services;
using Application.Recruitment.Features.Authenticator.DTOs;
using Application.Recruitment.Features.Authenticator.Handlers.Utils;
using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Kawader;

namespace Tawtheef.Infrastructure.Services;

public class MoiService(IMediator mediator, IUnitOfWork uow, IAppLogger log) : IMoiService
{
    public async Task<IResult<MOEPersonalInfo>> GetMoiPersonalInfoAsync(string qid, DateOnly expiryDate,
        CancellationToken cancellationToken) {
        var qidMasked = MoiUtils.MaskQid(qid);

        var request = await mediator.Send(
            new GetPersonalInformationByQidQuery(new CheckProfileMOI(qid, expiryDate)),
            cancellationToken);

        if (request.IsFailed) {
            log.Warning(
                "MOI personal info query failed. Qid={QidMasked} Errors={Errors}",
                qidMasked,
                string.Join(" | ", request.Errors.Select(e => e.Message)));

            return Result.Fail<MOEPersonalInfo>(request.Errors);
        }

        if (request.Value.NationalityCode != MoiUtils.QatarNationalityCode)
            return Result.Ok(request.Value);

        var allowLogin = await CheckIfAllowLoginAsync();
        return allowLogin
            ? Result.Ok(request.Value)
            : Result.Fail<MOEPersonalInfo>(ErrorsCodes.QatariPeopleNotAllowedLoginBeforeRegisterOnKawader);

        async Task<bool> CheckIfAllowLoginAsync()
        {
            var isKawaderUser = await uow.GetEntityRepository<KawaderQid>()
                .DbSet.AsNoTracking()
                .AnyAsync(x => x.Qid == qid, cancellationToken);

            log.Information(
                "Kawader allow-login check. Qid={QidMasked} IsKawaderUser={IsKawaderUser}",
                qidMasked,
                isKawaderUser);

            return isKawaderUser;
        }
    }
}


