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

public sealed class ReviseProfileLanguageDeleteHandler(IUnitOfWork uow) :
    IRequestHandler<ReviseProfileLanguageDeleteCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileLanguageDeleteCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        
        if(profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var repo = uow.GetEntityRepository<ProfileLanguage>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.Id && x.UserProfileId == profile.Id, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.LanguageProfileNotFound);

        var canDelete = await ReviewDeleteGuard.CanDeleteRowOrSectionAsync(
            uow,
            profile.Id,
            ProfileSection.Languages,
            cmd.Id,
            ct);

        if (!canDelete)
            return Result.Fail<Unit>(ErrorsCodes.AttachmentNotEditableInRevision);

        await repo.DeleteAsync(target);
        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Languages, ct, force: true);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}

