using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfilePersonalAttachmentsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IMediator mediator
) : ICommandHandler<ReviseProfilePersonalAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfilePersonalAttachmentsCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not (UserProfileStatus.InCreation or UserProfileStatus.RequiresUpdate))
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        // Validate step (if your validator expects these props already set, do it after updates)
        var vr = validationService.ValidateAttachments(profile);
        if (vr.IsFailed)
            return Result.Fail<Unit>(vr.Errors);

        // Resume
        if (cmd.Request.Resume is not null)
        {
            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Personal,
                meta: cmd.Request.Resume,
                file: cmd.Request.ResumeAttachment,
                currentProfileResourceId: profile.ResumeAttachmentId,
                folder: "resume",
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.ResumeAttachmentId = newId.Value;
        }

        // National card
        if (cmd.Request.NationalCard is not null)
        {
            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Personal,
                meta: cmd.Request.NationalCard,
                file: cmd.Request.NationalCardAttachment,
                currentProfileResourceId: profile.NationalCardId,
                folder: "national-card",
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.NationalCardId = newId.Value;
        }

        // Sponsor card (nested)
        if (cmd.Request.SponsorCard is not null)
        {
            if (profile.SponsorProfile is null)
                return Result.Fail<Unit>(ErrorsCodes.SponsorProfileNotFound);

            var current = profile.SponsorProfile.SponsorCardId;

            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Personal,
                meta: cmd.Request.SponsorCard,
                file: cmd.Request.SponsorCardAttachment,
                currentProfileResourceId: current,
                folder: "sponsor-card",
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.SponsorProfile.SponsorCardId = newId.Value;
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
