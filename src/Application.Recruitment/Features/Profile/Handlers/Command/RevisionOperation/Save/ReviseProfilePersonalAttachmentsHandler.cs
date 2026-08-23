using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfilePersonalAttachmentsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IMediator mediator
) : IRequestHandler<ReviseProfilePersonalAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfilePersonalAttachmentsCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        // Validate step (if your validator expects these props already set, do it after updates)
        var vr = validationService.ValidateAttachments(profile);
        if (vr.IsFailed)
            return Result.Fail<Unit>(vr.Errors);

        // Resume
        if (cmd.Request.Resume is not null)
        {
            var oldResourceId = profile.ResumeAttachmentId;
            if (oldResourceId == Guid.Empty || oldResourceId is null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var allowed = await reviewRepo.DbSet
                .AsNoTracking()
                .AnyAsync(r =>
                    r.UserProfileId == profile.Id &&
                    r.Section == ProfileSection.Prerequisites &&
                    r.TargetType == ReviewTargetType.Attachment &&
                    r.ResourceId == oldResourceId &&
                    (r.Status == ReviewStatus.NeedsCorrection ||
                     r.Status == ReviewStatus.Solved),
                    ct);

            if (!allowed)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Prerequisites,
                meta: cmd.Request.Resume,
                file: cmd.Request.ResumeAttachment,
                currentProfileResourceId: profile.ResumeAttachmentId,
                folder: ProfileFileCategories.Resume,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.ResumeAttachmentId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate || profile.Status == UserProfileStatus.Submitted)
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(uow, profile, ProfileSection.Prerequisites, oldResourceId, ct);
        }

        // National card
        if (cmd.Request.NationalCard is not null)
        {
            var oldResourceId = profile.NationalCardId;
            if (oldResourceId == Guid.Empty || oldResourceId is null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var allowed = await reviewRepo.DbSet
                .AsNoTracking()
                .AnyAsync(r =>
                    r.UserProfileId == profile.Id &&
                    r.Section == ProfileSection.Prerequisites &&
                    r.TargetType == ReviewTargetType.Attachment &&
                    r.ResourceId == oldResourceId &&
                    (r.Status == ReviewStatus.NeedsCorrection ||
                     r.Status == ReviewStatus.Solved),
                    ct);

            if (!allowed)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Prerequisites,
                meta: cmd.Request.NationalCard,
                file: cmd.Request.NationalCardAttachment,
                currentProfileResourceId: profile.NationalCardId,
                folder: ProfileFileCategories.NationalCard,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.NationalCardId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate || profile.Status == UserProfileStatus.Submitted)
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(uow, profile, ProfileSection.Prerequisites, oldResourceId, ct);
        }

        // Sponsor card (nested)
        if (cmd.Request.SponsorCard is not null)
        {
            if (profile.SponsorProfile is null)
                return Result.Fail<Unit>(ErrorsCodes.SponsorProfileNotFound);

            var current = profile.SponsorProfile.SponsorCardId;
            var oldResourceId = current;
            if (oldResourceId == Guid.Empty || oldResourceId is null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var allowed = await reviewRepo.DbSet
                .AsNoTracking()
                .AnyAsync(r =>
                    r.UserProfileId == profile.Id &&
                    r.Section == ProfileSection.Personal &&
                    r.TargetType == ReviewTargetType.Attachment &&
                    r.ResourceId == oldResourceId &&
                    (r.Status == ReviewStatus.NeedsCorrection ||
                     r.Status == ReviewStatus.Solved),
                    ct);

            if (!allowed)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Personal,
                meta: cmd.Request.SponsorCard,
                file: cmd.Request.SponsorCardAttachment,
                currentProfileResourceId: current,
                folder: ProfileFileCategories.SponsorCard,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.SponsorProfile.SponsorCardId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate || profile.Status == UserProfileStatus.Submitted)
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(uow, profile, ProfileSection.Personal, oldResourceId, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

