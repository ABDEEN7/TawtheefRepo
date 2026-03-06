using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Delete;

public sealed class ReviseProfileEducationDeleteHandler(IUnitOfWork uow) :
    IRequestHandler<ReviseProfileEducationDeleteCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileEducationDeleteCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        
        if(profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var qualificationRepo = uow.GetEntityRepository<Qualification>();
        var target = await qualificationRepo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.Id && x.UserProfileId == profile.Id, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.DegreeNotFound);

        var experiencesRepo = uow.GetEntityRepository<Experience>();
        var isLinkedToExperience = await experiencesRepo.DbSet
            .AnyAsync(x => x.QualificationId == target.Id && x.UserProfileId == profile.Id, ct);

        if (isLinkedToExperience)
            return Result.Fail<Unit>(ErrorsCodes.DegreeLinkedToExperience);

        await qualificationRepo.DeleteAsync(target);
        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Qualifications, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}

