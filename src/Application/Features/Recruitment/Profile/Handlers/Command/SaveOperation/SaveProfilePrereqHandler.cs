using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfilePrereqHandler(
    IUnitOfWork uow,
    IMediator mediator,
    UserManager<User> userManager,
    IProfileStepValidationService validationService)
    : IRequestHandler<SaveProfilePrereqCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfilePrereqCommand cmd, CancellationToken ct)
    {
        var profileResult = await UserProfileLoader.GetOrCreateAsync(uow,userManager, cmd.UserId, ct);
        if (profileResult.IsFailed) return Result.Fail<Unit>(profileResult.Errors);

        var profile = profileResult.Value;
        var validationResult = validationService.ValidatePrerequisites(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;
        if (profile.Status is not UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

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
        var nextCandidateTypeId = request.CandidateTypeId;
        var snapshot = this with
        {
            CandidateTypeId = nextCandidateTypeId,
            TargetEntityId = request.TargetEntityId,
            OfficeId = request.OfficeId ?? OfficeId,
            QidExpiry = request.QIDExpiry ?? QidExpiry,
            ResumeAttachmentId = resumeAttachmentId ?? ResumeAttachmentId,
            NationalCardId = nationalCardId ?? NationalCardId
        };

        if (RequiresBirthCertificate(nextCandidateTypeId))
        {
            snapshot = snapshot with { BirthCertificateId = birthCertificateId ?? BirthCertificateId };
        }
        else
        {
            snapshot = snapshot with { BirthCertificateId = null };
        }

        if (RequiresMarriageCertificate(nextCandidateTypeId))
        {
            snapshot = snapshot with { MarriageCertificateId = marriageCertificateId ?? MarriageCertificateId };
        }
        else
        {
            snapshot = snapshot with { MarriageCertificateId = null };
        }

        if (!RequiresOffice(nextCandidateTypeId))
        {
            snapshot = snapshot with { OfficeId = null };
        }

        if (!RequiresNationalAddress(nextCandidateTypeId))
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
