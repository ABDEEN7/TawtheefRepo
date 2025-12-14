using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfilePrereqHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileReviewService reviewService,
    IProfileStepValidationService validationService)
    : IRequestHandler<SaveProfilePrereqCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfilePrereqCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .Include(p => p.SponsorProfile)
            .Include(p => p.ResidenceAddress)
            .SingleOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            profile = new UserProfile
            {
                UserId  = cmd.UserId
            };
            await profileRepo.AddAsync(profile);
        }

        if (profile.Status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var trackChanges = profile.Status == UserProfileStatus.Approved;

        var oldCandidateTypeId = profile.CandidateTypeId;
        var oldTargetEntityId = profile.TargetEntityId;
        var oldOfficeId = profile.OfficeId;
        var oldQidExpiry = profile.QIDExpiry;

        var oldSnapshot = BuildPrereqSnapshot(profile);

        var validationResult = validationService.ValidatePrerequisites(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;

        profile.CandidateTypeId = r.CandidateTypeId;
        profile.TargetEntityId  = r.TargetEntityId;

        var needsSponsor = profile.CandidateTypeId == CandidateTypeIds.ResidentQatar;
        var needsBirthCertificate = profile.CandidateTypeId == CandidateTypeIds.SonOfQatariMother;
        var needsMarriageCertificate = profile.CandidateTypeId == CandidateTypeIds.WifeOfQatari;
        var needsOffice = profile.CandidateTypeId == CandidateTypeIds.NonQatari || profile.CandidateTypeId == CandidateTypeIds.GCC;
        var requiresNationalAddress = profile.CandidateTypeId != CandidateTypeIds.NonQatari
            && profile.CandidateTypeId != CandidateTypeIds.GCC;

        profile.OfficeId = needsOffice ? r.OfficeId : null;
        profile.QIDExpiry = requiresNationalAddress ? r.QIDExpiry ?? profile.QIDExpiry : null;

        // CV
        var cvResult = await UploadIfNeededAsync(r.CvFile, profile.ResumeAttachmentId, "cv");
        if (cvResult.IsFailed)
            return Result.Fail<Unit>(cvResult.Errors);
        profile.ResumeAttachmentId = cvResult.Value;

        // ID
        var idResult = await UploadIfNeededAsync(r.IdFile, profile.NationalCardId, "national-id");
        if (idResult.IsFailed)
            return Result.Fail<Unit>(idResult.Errors);
        profile.NationalCardId = idResult.Value;

        // Birth Certificate
        if (needsBirthCertificate)
        {
            var birthResult = await UploadIfNeededAsync(r.BirthCertificateFile, profile.BirthdayCertificateId, "birth-certificate");
            if (birthResult.IsFailed)
                return Result.Fail<Unit>(birthResult.Errors);
            profile.BirthdayCertificateId = birthResult.Value;
        }
        else
        {
            profile.BirthdayCertificateId = null;
        }

        // Marriage Certificate
        if (needsMarriageCertificate)
        {
            var marriageResult = await UploadIfNeededAsync(r.MarriageCertificateFile, profile.MarriageCertificateId, "marriage-certificate");
            if (marriageResult.IsFailed)
                return Result.Fail<Unit>(marriageResult.Errors);
            profile.MarriageCertificateId = marriageResult.Value;
        }
        else
        {
            profile.MarriageCertificateId = null;
        }

        CleanCandidateTypeDependents();

        var newSnapshot = BuildPrereqSnapshot(profile);

        if (trackChanges)
        {
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.CandidateTypeId), ct, oldCandidateTypeId, profile.CandidateTypeId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.TargetEntityId), ct, oldTargetEntityId, profile.TargetEntityId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Contact, nameof(UserProfile.QIDExpiry), ct, oldQidExpiry, profile.QIDExpiry);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.OfficeId), ct, oldOfficeId, profile.OfficeId);
            await reviewService.TouchSectionAsync(profile.Id, ProfileSection.Personal, ct, oldSnapshot, newSnapshot);
            if (profile.ResumeAttachmentId is not null)
                await reviewService.TouchAttachmentAsync(profile.Id, ProfileSection.Attachments, "Resume", profile.ResumeAttachmentId.Value, ct);
            if (profile.NationalCardId is not null)
                await reviewService.TouchAttachmentAsync(profile.Id, ProfileSection.Attachments, "NationalCard", profile.NationalCardId.Value, ct);
            if (profile.BirthdayCertificateId is not null)
                await reviewService.TouchAttachmentAsync(profile.Id, ProfileSection.Attachments, "BirthCertificate", profile.BirthdayCertificateId.Value, ct);
            if (profile.MarriageCertificateId is not null)
                await reviewService.TouchAttachmentAsync(profile.Id, ProfileSection.Attachments, "MarriageCertificate", profile.MarriageCertificateId.Value, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        void CleanCandidateTypeDependents()
        {
            if (!needsSponsor)
            {
                profile.SponsorProfile = null;
                profile.SponsorProfileId = null;
            }

            if (!requiresNationalAddress)
            {
                profile.ResidenceAddress = null;
                profile.ResidenceAddressId = null;
                profile.ResidenceAddressCertificateId = null;
            }
            else
            {
                profile.Address = null;
            }

            if (!needsOffice)
            {
                profile.Office = null;
                profile.OfficeId = null;
            }
        }

        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId, string category)
        {
            if (file is null || file.Length == 0)
                return Result.Ok(existingId);

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, category, file, false, ct);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                ct);
            if (uploadResult.IsFailed)
            return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }

        static object BuildPrereqSnapshot(UserProfile profileEntity) => new
        {
            profileEntity.CandidateTypeId,
            profileEntity.TargetEntityId,
            profileEntity.OfficeId,
            profileEntity.QIDExpiry,
            profileEntity.ResumeAttachmentId,
            profileEntity.NationalCardId,
            profileEntity.BirthdayCertificateId,
            profileEntity.MarriageCertificateId
        };
    }
}
