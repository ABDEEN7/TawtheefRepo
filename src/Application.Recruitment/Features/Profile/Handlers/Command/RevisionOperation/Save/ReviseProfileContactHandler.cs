using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Utils;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfileContactHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : IRequestHandler<ReviseProfileContactCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileContactCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var validationResult = validationService.ValidateContact(profile, cmd.Request.Address,
            cmd.Request.NationalAddress is null
                ? null
                : new(cmd.Request.NationalAddress.Zone, cmd.Request.NationalAddress.Street,
                    cmd.Request.NationalAddress.Building,
                    cmd.Request.NationalAddress.Unit,
                    cmd.Request.NationalAddress.NationalAddressFileName ??
                    cmd.Request.NationalAddress.NationalAddress?.FileName));

        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var contactReviewItemExists = await reviewRepo.DbSet
            .AsNoTracking()
            .AnyAsync(reviewItem =>
                    reviewItem.UserProfileId == profile.Id &&
                    reviewItem.ProfileChangeId == null &&
                    !reviewItem.IsDeleted &&
                    reviewItem.Section == ProfileSection.Contact &&
                    reviewItem.TargetType == ReviewTargetType.Field &&
                    reviewItem.FieldPath == ProfileReviewConstants.FieldPaths.SectionData &&
                    (reviewItem.Status == ReviewStatus.NeedsCorrection ||
                     reviewItem.Status == ReviewStatus.Rejected ||
                     reviewItem.Status == ReviewStatus.Solved),
                ct);

        if (!contactReviewItemExists)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var needsOffice = ProfileValidatorUtils.RequiresOffice(profile.CandidateTypeId, profile.Provider);

        Guid? officeId = null;
        if (needsOffice)
        {
            officeId = await uow.GetEntityRepository<Office>().DbSet.AsNoTracking()
                .Where(o => o.CountryId == r.ResidenceCountryId)
                .OrderBy(o => o.DisplayOrder)
                .ThenBy(o => o.NameEn)
                .Select(o => (Guid?)o.Id)
                .FirstOrDefaultAsync(ct);

            if (officeId is null)
                return Result.Fail<Unit>(ErrorsCodes.OfficeRequired);
        }

        profile.ResidenceCountryId = r.ResidenceCountryId;
        profile.InterviewLocationId = r.InterviewLocationId;
        profile.Address = r.Address;
        profile.OfficeId = needsOffice ? officeId : null;

        if (r.NationalAddress is not null)
        {
            var oldResourceId = profile.ResidenceAddress?.CertificateId;
            if (HasFile(r.NationalAddress.NationalAddress))
            {
                var editable = await EnsureAttachmentEditableAsync(oldResourceId);
                if (editable.IsFailed)
                    return Result.Fail<Unit>(editable.Errors);
            }

            var idResult = await UploadIfNeededAsync(r.NationalAddress.NationalAddress, oldResourceId);
            if (idResult.IsFailed)
                return Result.Fail<Unit>(idResult.Errors);

            if (idResult.Value is null || idResult.Value == Guid.Empty)
                return Result.Fail<Unit>(ErrorsCodes.NationalAddressCertificateRequired);

            if (profile.ResidenceAddress is null)
            {
                profile.ResidenceAddress =
                    ResidenceAddress.Create(r.NationalAddress.Building, r.NationalAddress.Street,
                        r.NationalAddress.Zone, r.NationalAddress.Unit, idResult.Value.Value);
            }
            else
            {
                profile.ResidenceAddress.ZoneNo = r.NationalAddress.Zone;
                profile.ResidenceAddress.StreetNo = r.NationalAddress.Street;
                profile.ResidenceAddress.BuildingNo = r.NationalAddress.Building;
                profile.ResidenceAddress.UnitNo = r.NationalAddress.Unit;
                profile.ResidenceAddress.CertificateId = idResult.Value!.Value;
            }

            if (HasFile(r.NationalAddress.NationalAddress))
            {
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                    uow, profile, ProfileSection.Contact, oldResourceId, ct);
            }
        }

        await ProfileReviewItemSync.EnsureNationalAddressAttachmentItemAsync(uow, profile, ct);
        await ReviewItemSaveHelper.MarkSectionDataSolvedAsync(uow, profile, ProfileSection.Contact, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId)
        {
            if (file is null || file.Length == 0)
                return existingId is null || existingId == Guid.Empty
                    ? Result.Fail<Guid?>(ErrorsCodes.NationalAddressCertificateRequired)
                    : Result.Ok(existingId);

            var uploadPath = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId,
                ProfileFileCategories.NationalAddress, file, false, ct);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }

        static bool HasFile(IFormFile? file) => file is { Length: > 0 };

        async Task<Result> EnsureAttachmentEditableAsync(Guid? resourceId)
        {
            if (resourceId is null || resourceId == Guid.Empty)
                return Result.Ok();

            var allowed = await reviewRepo.DbSet.AsNoTracking().AnyAsync(item =>
                    item.UserProfileId == profile.Id &&
                    item.ProfileChangeId == null &&
                    !item.IsDeleted &&
                    item.Section == ProfileSection.Contact &&
                    item.TargetType == ReviewTargetType.Attachment &&
                    item.ResourceId == resourceId &&
                    (item.Status == ReviewStatus.NeedsCorrection || item.Status == ReviewStatus.Rejected ||
                     item.Status == ReviewStatus.Solved),
                ct);

            if (allowed)
                return Result.Ok();

            return Result.Fail(new Error("Forbidden")
                .WithMetadata("Code", ErrorsCodes.AttachmentNotEditableInRevision)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
        }
    }
}
