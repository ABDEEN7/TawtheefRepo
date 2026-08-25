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

public sealed class ReviseProfileContactAttachmentsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IMediator mediator
) : IRequestHandler<ReviseProfileContactAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileContactAttachmentsCommand cmd, CancellationToken ct)
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

        if (profile.ResidenceAddress is null)
            return Result.Fail<Unit>(ErrorsCodes.ResidenceAddressNotFound);

        if (cmd.Request.ResidenceAddress is not null)
        {
            var current = profile.ResidenceAddress.CertificateId;
            var oldResourceId = current;

            var reviewItemExists = await reviewRepo.DbSet
                .AsNoTracking()
                .AnyAsync(r =>
                    r.UserProfileId == profile.Id &&
                    r.ProfileChangeId == null &&
                    !r.IsDeleted &&
                    r.Section == ProfileSection.Contact &&
                    r.TargetType == ReviewTargetType.Attachment &&
                    r.ResourceId == oldResourceId &&
                    (r.Status == ReviewStatus.NeedsCorrection ||
                     r.Status == ReviewStatus.Rejected ||
                     r.Status == ReviewStatus.Solved),
                    ct);

            if (!reviewItemExists)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Contact,
                meta: cmd.Request.ResidenceAddress,
                file: cmd.Request.ResidenceAddressCertificate,
                currentProfileResourceId: current,
                folder: ProfileFileCategories.ResidenceCertificate,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.ResidenceAddress.CertificateId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate || profile.Status == UserProfileStatus.Submitted)
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(uow, profile, ProfileSection.Contact, oldResourceId, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

