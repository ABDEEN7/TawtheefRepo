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
using Tawtheef.Domain.Utils;

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
        var profileResult = await UserProfileLoader.GetSummaryAsync(uow, cmd.UserId, ct);
        if (profileResult.IsFailed) return Result.Fail<Unit>(profileResult.Errors);
        var profile = profileResult.Value;
        if (profile is null)
        {
            var user = await userManager.FindByIdAsync($"{cmd.UserId}");
            if(user is null) return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
            var logins = await userManager.GetLoginsAsync(user);
            var providerName = logins.FirstOrDefault()?.ProviderDisplayName?.Replace(" ", "") ?? "Unknown";
            profile = new UserProfile
            {
                UserId = cmd.UserId,
                Provider = providerName,
                Status = UserProfileStatus.InCreation,
                CandidateTypeId = cmd.Request.CandidateTypeId,
                TargetEntityId = cmd.Request.TargetEntityId
            };
            await uow.GetEntityRepository<UserProfile>().AddAsync(profile);
        }
        
        var validationResult = validationService.ValidatePrerequisites(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;
        if (profile.Status is not UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        profile.CandidateTypeId = r.CandidateTypeId;
        profile.TargetEntityId  = r.TargetEntityId;

        var needsSponsor = ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider);
        var needsBirthCertificate = ProfileValidatorUtils.RequiresBirthCertificate(profile.CandidateTypeId);
        var needsMarriageCertificate = ProfileValidatorUtils.RequiresMarriageCertificate(profile.CandidateTypeId);
        var requiresNationalAddress = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);

        profile.QIDExpiry = requiresNationalAddress ? r.QIDExpiry ?? profile.QIDExpiry : null;

        // CV
        var cvResult = await UploadIfNeededAsync(r.CvFile, profile.ResumeAttachmentId, "cv");
        if (cvResult.IsFailed)  return Result.Fail<Unit>(cvResult.Errors);
        profile.ResumeAttachmentId = cvResult.Value;

        // ID
        var idResult = await UploadIfNeededAsync(r.IdFile, profile.NationalCardId, "national-id");
        if (idResult.IsFailed) return Result.Fail<Unit>(idResult.Errors);
        profile.NationalCardId = idResult.Value;

        // Birth Certificate
        if (needsBirthCertificate)
        {
            var birthResult = await UploadIfNeededAsync(r.BirthCertificateFile, profile.BirthdayCertificateId, "birth-certificate");
            if (birthResult.IsFailed) return Result.Fail<Unit>(birthResult.Errors);
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
            if (marriageResult.IsFailed) return Result.Fail<Unit>(marriageResult.Errors);
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
    }
}
