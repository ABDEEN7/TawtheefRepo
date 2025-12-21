using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperations;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfilePrereqHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService)
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

        var validationResult = validationService.ValidatePrerequisites(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;
        if (profile.Status is not UserProfileStatus.InCreation)
        {
            var currentSnapshot = PrereqSectionSnapshot.From(profile);

            var cvUpload = await UploadIfNeededAsync(r.CvFile, profile.ResumeAttachmentId, "cv");
            if (cvUpload.IsFailed) return Result.Fail<Unit>(cvUpload.Errors);
            var idUpload = await UploadIfNeededAsync(r.IdFile, profile.NationalCardId, "national-id");
            if (idUpload.IsFailed) return Result.Fail<Unit>(idUpload.Errors);
            var birthUpload = await UploadIfNeededAsync(r.BirthCertificateFile, profile.BirthdayCertificateId, "birth-certificate");
            if (birthUpload.IsFailed) return Result.Fail<Unit>(birthUpload.Errors);
            var marriageUpload = await UploadIfNeededAsync(r.MarriageCertificateFile, profile.MarriageCertificateId, "marriage-certificate");
            if (marriageUpload.IsFailed) return Result.Fail<Unit>(marriageUpload.Errors);

            var nextSnapshot = currentSnapshot.ApplyRequest(
                r,
                cvUpload.Value,
                idUpload.Value,
                birthUpload.Value,
                marriageUpload.Value
            );

            if (nextSnapshot != currentSnapshot)
            {
                await reviewService.TouchSectionAsync(profile.Id, ProfileSection.Prerequisites, cmd.UserId, ct, currentSnapshot, nextSnapshot);
                await uow.SaveChangesAsync(ct);
            }

            return Result.Ok(Unit.Value);
        }

        profile.CandidateTypeId = r.CandidateTypeId;
        profile.TargetEntityId  = r.TargetEntityId;

        var needsSponsor = PrereqSectionSnapshot.RequiresSponsor(profile.CandidateTypeId);
        var needsBirthCertificate = PrereqSectionSnapshot.RequiresBirthCertificate(profile.CandidateTypeId);
        var needsMarriageCertificate = PrereqSectionSnapshot.RequiresMarriageCertificate(profile.CandidateTypeId);
        var needsOffice = PrereqSectionSnapshot.RequiresOffice(profile.CandidateTypeId);
        var requiresNationalAddress = PrereqSectionSnapshot.RequiresNationalAddress(profile.CandidateTypeId);

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
    }
}

file sealed record PrereqSectionSnapshot
{
    public Guid? CandidateTypeId { get; init; }
    public Guid? TargetEntityId { get; init; }
    public Guid? OfficeId { get; init; }
    public DateOnly? QidExpiry { get; init; }
    public Guid? ResumeAttachmentId { get; init; }
    public Guid? NationalCardId { get; init; }
    public Guid? BirthCertificateId { get; init; }
    public Guid? MarriageCertificateId { get; init; }

    public static PrereqSectionSnapshot From(UserProfile profile) => new()
    {
        CandidateTypeId = profile.CandidateTypeId,
        TargetEntityId = profile.TargetEntityId,
        OfficeId = profile.OfficeId,
        QidExpiry = profile.QIDExpiry,
        ResumeAttachmentId = profile.ResumeAttachmentId,
        NationalCardId = profile.NationalCardId,
        BirthCertificateId = profile.BirthdayCertificateId,
        MarriageCertificateId = profile.MarriageCertificateId
    };

    public PrereqSectionSnapshot ApplyRequest(
        SaveProfilePrereqRequest request,
        Guid? resumeAttachmentId,
        Guid? nationalCardId,
        Guid? birthCertificateId,
        Guid? marriageCertificateId)
    {
        var nextCandidateTypeId = request.CandidateTypeId ?? CandidateTypeId;
        var snapshot = this with
        {
            CandidateTypeId = nextCandidateTypeId,
            TargetEntityId = request.TargetEntityId ?? TargetEntityId,
            OfficeId = request.OfficeId ?? OfficeId,
            QidExpiry = request.QIDExpiry ?? QidExpiry,
            ResumeAttachmentId = resumeAttachmentId ?? ResumeAttachmentId,
            NationalCardId = nationalCardId ?? NationalCardId
        };

        if (PrereqSectionSnapshot.RequiresBirthCertificate(nextCandidateTypeId))
        {
            snapshot = snapshot with { BirthCertificateId = birthCertificateId ?? BirthCertificateId };
        }
        else
        {
            snapshot = snapshot with { BirthCertificateId = null };
        }

        if (PrereqSectionSnapshot.RequiresMarriageCertificate(nextCandidateTypeId))
        {
            snapshot = snapshot with { MarriageCertificateId = marriageCertificateId ?? MarriageCertificateId };
        }
        else
        {
            snapshot = snapshot with { MarriageCertificateId = null };
        }

        if (!PrereqSectionSnapshot.RequiresOffice(nextCandidateTypeId))
        {
            snapshot = snapshot with { OfficeId = null };
        }

        if (!PrereqSectionSnapshot.RequiresNationalAddress(nextCandidateTypeId))
        {
            snapshot = snapshot with { QidExpiry = null };
        }

        return snapshot;
    }

    public static bool RequiresSponsor(Guid? candidateTypeId) =>
        candidateTypeId == CandidateTypeIds.ResidentQatar;

    public static bool RequiresBirthCertificate(Guid? candidateTypeId) =>
        candidateTypeId == CandidateTypeIds.SonOfQatariMother;

    public static bool RequiresMarriageCertificate(Guid? candidateTypeId) =>
        candidateTypeId == CandidateTypeIds.WifeOfQatari;

    public static bool RequiresOffice(Guid? candidateTypeId) =>
        candidateTypeId == CandidateTypeIds.NonQatari || candidateTypeId == CandidateTypeIds.GCC;

    public static bool RequiresNationalAddress(Guid? candidateTypeId) =>
        candidateTypeId != CandidateTypeIds.NonQatari && candidateTypeId != CandidateTypeIds.GCC;
}
