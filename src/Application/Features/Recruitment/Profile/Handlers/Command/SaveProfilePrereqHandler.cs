using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;


public sealed class SaveProfilePrereqHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveProfilePrereqCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfilePrereqCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            profile = new UserProfile
            {
                UserId  = cmd.UserId,
                IsDraft = true
            };
            await profileRepo.AddAsync(profile);
        }

        var r = cmd.Request;

        profile.CandidateTypeId = r.CandidateTypeId;
        profile.TargetEntityId  = r.TargetEntityId;
        profile.BirthdayCertificateId     = r.BirthCertResourceId;
        profile.MarriageCertificateId = r.MarriageCertResourceId;
        
        profile.NationalCardId       = r.IdResourceId;
        profile.ResumeAttachmentId       = r.CvResourceId;

        profile.IsDraft = true;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
