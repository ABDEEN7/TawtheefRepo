using Microsoft.EntityFrameworkCore;
using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfilePrereqAttachmentsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IMediator mediator
) : IRequestHandler<ReviseProfilePrereqAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfilePrereqAttachmentsCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);


        if (profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var vr = validationService.ValidateAttachments(profile);
        if (vr.IsFailed)
            return Result.Fail<Unit>(vr.Errors);

        if (cmd.Request.Birthday is not null)
        {
            var oldResourceId = profile.BirthdayCertificateId;

            if (oldResourceId is null || oldResourceId == Guid.Empty)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var reviewAllowed = await reviewRepo.DbSet.AnyAsync(x =>
                x.UserProfileId == profile.Id &&
                x.Section == ProfileSection.Prerequisites &&
                x.TargetType == ReviewTargetType.Attachment &&
                x.ResourceId == oldResourceId &&
                (x.Status == ReviewStatus.NeedsCorrection ||
                 x.Status == ReviewStatus.Solved),
                ct);

            if (!reviewAllowed)
                return Result.Fail<Unit>(ErrorsCodes.AttachmentNotEditableInRevision);



            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Prerequisites,
                meta: cmd.Request.Birthday,
                file: cmd.Request.BirthdayCertificate,
                currentProfileResourceId: profile.BirthdayCertificateId,
                folder: ProfileFileCategories.BirthCertificate,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.BirthdayCertificateId = newId.Value;
            if (oldResourceId.HasValue && oldResourceId.Value != Guid.Empty)
            {
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                    uow,
                    profile,
                    ProfileSection.Prerequisites,
                    oldResourceId.Value,
                    ct);
            }
        }

        if (cmd.Request.Marriage is not null)
        {
            var oldResourceId = profile.MarriageCertificateId;

            if (oldResourceId is null || oldResourceId == Guid.Empty)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var reviewAllowed = await reviewRepo.DbSet
                .Where(x =>
                    x.UserProfileId == profile.Id &&
                    x.Section == ProfileSection.Prerequisites &&
                    x.TargetType == ReviewTargetType.Attachment &&
                    x.ResourceId == oldResourceId &&
                    (x.Status == ReviewStatus.NeedsCorrection ||
                     x.Status == ReviewStatus.Solved))
                .AnyAsync(ct);

            if (!reviewAllowed)
                return Result.Fail<Unit>(ErrorsCodes.AttachmentNotEditableInRevision);

            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Prerequisites,
                meta: cmd.Request.Marriage,
                file: cmd.Request.MarriageCertificate,
                currentProfileResourceId: profile.MarriageCertificateId,
                folder: ProfileFileCategories.MarriageCertificate,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.MarriageCertificateId = newId.Value;
            if (oldResourceId.HasValue && oldResourceId.Value != Guid.Empty)
            {
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                    uow,
                    profile,
                    ProfileSection.Prerequisites,
                    oldResourceId.Value,
                    ct);
            }
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

