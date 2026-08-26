using Microsoft.EntityFrameworkCore;
using Application.Recruitment.Features.Profile.Policies;
using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Utils;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfilePrereqHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService)
    : IRequestHandler<ReviseProfilePrereqCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfilePrereqCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        
        var validationResult = validationService.ValidatePrerequisites(profile, cmd.Request.CandidateTypeId);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;
        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var prerequisiteSectionDataIsActionable = await reviewRepo.DbSet
            .AsNoTracking()
            .AnyAsync(item =>
                item.UserProfileId == profile.Id &&
                item.ProfileChangeId == null &&
                !item.IsDeleted &&
                item.Section == ProfileSection.Prerequisites &&
                item.TargetType == ReviewTargetType.Field &&
                item.FieldPath == ProfileReviewConstants.FieldPaths.SectionData &&
                (item.Status == ReviewStatus.NeedsCorrection ||
                 item.Status == ReviewStatus.Rejected ||
                 item.Status == ReviewStatus.Solved),
                ct);

        if (!prerequisiteSectionDataIsActionable)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var isLockedProvider = VerifiedIdentityProviders.IsLockedProvider(profile.Provider);
        var previouslyRequiredSponsor = ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider);

        if (!isLockedProvider || !CandidateTypeIds.IsVerifiedIdentityLocked(profile.CandidateTypeId))
            profile.CandidateTypeId = r.CandidateTypeId;

        profile.TargetEntityId  = r.TargetEntityId;

        var requirements = ConditionalRequirements.For(profile.CandidateTypeId, profile.Provider);

        if (!isLockedProvider)
        {
            profile.QIDExpiry = requirements.RequiresNationalAddress ? r.QIDExpiry ?? profile.QIDExpiry : null;
        }
        else if (requirements.RequiresNationalAddress)
        {
            profile.QIDExpiry = profile.QIDExpiry ?? r.QIDExpiry;
        }

        if (HasFile(r.CvFile))
        {
            var oldResourceId = profile.ResumeAttachmentId;
            var editableCv = await EnsureAttachmentEditableAsync(oldResourceId);
            if (editableCv.IsFailed)
                return Result.Fail<Unit>(editableCv.Errors);

            var cvResult = await UploadIfNeededAsync(r.CvFile, profile.ResumeAttachmentId, ProfileFileCategories.Cv);
            if (cvResult.IsFailed)
                return Result.Fail<Unit>(cvResult.Errors);
            profile.ResumeAttachmentId = cvResult.Value;
            await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                uow, profile, ProfileSection.Prerequisites, oldResourceId, ct);
        }

        if (HasFile(r.IdFile))
        {
            var oldResourceId = profile.NationalCardId;
            var editableId = await EnsureAttachmentEditableAsync(oldResourceId);
            if (editableId.IsFailed)
                return Result.Fail<Unit>(editableId.Errors);

            var idResult = await UploadIfNeededAsync(r.IdFile, profile.NationalCardId, ProfileFileCategories.NationalId);
            if (idResult.IsFailed)
                return Result.Fail<Unit>(idResult.Errors);
            profile.NationalCardId = idResult.Value;
            await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                uow, profile, ProfileSection.Prerequisites, oldResourceId, ct);
        }

        // Birth Certificate
        if (requirements.RequiresBirthCertificate && HasFile(r.BirthCertificateFile))
        {
            var oldResourceId = profile.BirthdayCertificateId;
            var editable = await EnsureAttachmentEditableAsync(oldResourceId);
            if (editable.IsFailed)
                return Result.Fail<Unit>(editable.Errors);
            var birthResult = await UploadIfNeededAsync(r.BirthCertificateFile, profile.BirthdayCertificateId, ProfileFileCategories.BirthCertificate);
            if (birthResult.IsFailed) return Result.Fail<Unit>(birthResult.Errors);
            profile.BirthdayCertificateId = birthResult.Value;
            await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                uow, profile, ProfileSection.Prerequisites, oldResourceId, ct);
        }

        // Marriage Certificate
        if (requirements.RequiresMarriageCertificate && HasFile(r.MarriageCertificateFile))
        {
            var oldResourceId = profile.MarriageCertificateId;
            var editable = await EnsureAttachmentEditableAsync(oldResourceId);
            if (editable.IsFailed)
                return Result.Fail<Unit>(editable.Errors);

            var marriageResult = await UploadIfNeededAsync(r.MarriageCertificateFile, profile.MarriageCertificateId, ProfileFileCategories.MarriageCertificate);
            if (marriageResult.IsFailed) return Result.Fail<Unit>(marriageResult.Errors);
            profile.MarriageCertificateId = marriageResult.Value;
            await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                uow, profile, ProfileSection.Prerequisites, oldResourceId, ct);
        }

        var removedSponsor = previouslyRequiredSponsor && !requirements.RequiresSponsor
            ? profile.SponsorProfile
            : null;
        var removedResidenceAddress = !requirements.RequiresNationalAddress
            ? profile.ResidenceAddress
            : null;
        CleanCandidateTypeDependents();
        if (removedSponsor is not null)
            await uow.GetEntityRepository<SponsorProfile>().DeleteAsync(removedSponsor);
        if (removedResidenceAddress is not null)
            await uow.GetEntityRepository<ResidenceAddress>().DeleteAsync(removedResidenceAddress);

        if (!previouslyRequiredSponsor &&
            requirements.RequiresSponsor &&
            SponsorRequiresCandidateWork())
        {
            await ReviewItemSaveHelper.ReopenSectionDataForCorrectionAsync(
                uow,
                profile,
                ProfileSection.Personal,
                ct);
        }
        else if (previouslyRequiredSponsor && !requirements.RequiresSponsor)
        {
            await ReviewItemSaveHelper.MarkSystemAppliedSectionDataSolvedAsync(
                uow,
                profile,
                ProfileSection.Personal,
                ct);
        }

        await ProfileReviewItemSync.EnsurePrerequisiteAttachmentItemsAsync(uow, profile, ct);
        await ReviewItemSaveHelper.MarkSectionDataSolvedAsync(uow, profile, ProfileSection.Prerequisites, ct);
        var result = await uow.SaveChangesAsync(ct);
        return result == 0 ? Result.Fail<Unit>(ErrorsCodes.NoChangesMade) : Result.Ok(Unit.Value);

        void CleanCandidateTypeDependents()
        {
            if (!requirements.RequiresBirthCertificate)
            {
                profile.BirthdayCertificateId = null;
            }

            if (!requirements.RequiresMarriageCertificate)
            {
                profile.MarriageCertificateId = null;
            }

            if (!requirements.RequiresSponsor)
            {
                profile.SponsorProfile = null;
                profile.SponsorProfileId = null;
            }

            if (!requirements.RequiresNationalAddress)
            {
                profile.ResidenceAddress = null;
                profile.ResidenceAddressId = null;
            }
            else
            {
                profile.Address = null;
            }

            if (!requirements.RequiresOffice)
                profile.OfficeId = null;
        }

        bool SponsorRequiresCandidateWork()
        {
            var sponsor = profile.SponsorProfile;
            return sponsor is null ||
                   sponsor.SponsorTypeId == Guid.Empty ||
                   string.IsNullOrWhiteSpace(sponsor.SponsorName) ||
                   string.IsNullOrWhiteSpace(sponsor.SponsorNumber) ||
                   sponsor.QIDExpiry is null ||
                   sponsor.SponsorCardId is null ||
                   sponsor.SponsorCardId == Guid.Empty;
        }

        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId, string category)
        {
            if (file is null || file.Length == 0) 
                if (existingId is null)
                    return Result.Fail<Guid?>(ErrorsCodes.UploadFailed);
                else
                    return Result.Ok(existingId);

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, category, file, false, ct);
            var uploadResult = await mediator.Send(new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file), ct);
            if (uploadResult.IsFailed) return Result.Fail<Guid?>(uploadResult.Errors);
            if (uploadResult.Value?.ResourceId is null || uploadResult.Value?.ResourceId == Guid.Empty) 
                return Result.Fail<Guid?>(ErrorsCodes.UploadFailed);
            
            return Result.Ok<Guid?>(uploadResult.Value!.ResourceId);
        }

        static bool HasFile(IFormFile? file) => file is { Length: > 0 };

        async Task<Result> EnsureAttachmentEditableAsync(Guid? resourceId)
        {
            if (resourceId is null || resourceId == Guid.Empty)
                return Result.Ok(); // no existing attachment = nothing to check

            var allowed = await reviewRepo.DbSet
                .Where(x =>
                    x.UserProfileId == profile.Id &&
                    x.ProfileChangeId == null &&
                    !x.IsDeleted &&
                    x.Section == ProfileSection.Prerequisites &&
                    x.TargetType == ReviewTargetType.Attachment &&
                    x.ResourceId == resourceId &&
                    (x.Status == ReviewStatus.NeedsCorrection ||
                     x.Status == ReviewStatus.Rejected ||
                     x.Status == ReviewStatus.Solved))
                .AnyAsync(ct);

            if (!allowed)
            {
                return Result.Fail(new Error("Forbidden")
                    .WithMetadata("Code", ErrorsCodes.AttachmentNotEditableInRevision)
                    .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
            }

            return Result.Ok();
        }


    }

    private readonly record struct ConditionalRequirements(
        bool RequiresSponsor,
        bool RequiresBirthCertificate,
        bool RequiresMarriageCertificate,
        bool RequiresNationalAddress,
        bool RequiresOffice)
    {
        public static ConditionalRequirements For(Guid? candidateTypeId, string provider) => new(
            ProfileValidatorUtils.RequiresSponsor(candidateTypeId, provider),
            ProfileValidatorUtils.RequiresBirthCertificate(candidateTypeId),
            ProfileValidatorUtils.RequiresMarriageCertificate(candidateTypeId),
            ProfileValidatorUtils.RequiresNationalAddress(candidateTypeId, provider),
            ProfileValidatorUtils.RequiresOffice(candidateTypeId, provider));
    }
}


