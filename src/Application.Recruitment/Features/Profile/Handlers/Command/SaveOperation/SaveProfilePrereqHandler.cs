using Application.Recruitment.Features.Profile.Command.SaveOperation;
using Application.Recruitment.Features.Profile.Policies;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Utils;

namespace Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfilePrereqHandler(
    IUnitOfWork uow,
    IMediator mediator,
    UserManager<User> userManager)
    : IRequestHandler<SaveProfilePrereqCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfilePrereqCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
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
            await uow.GetEntityRepository<UserProfile>().AddAsync(profile, ct);
        }

        var r = cmd.Request;
        if (profile.Status != UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var isLockedProvider = VerifiedIdentityProviders.IsLockedProvider(profile.Provider);

        if (!isLockedProvider || !CandidateTypeIds.IsVerifiedIdentityLocked(profile.CandidateTypeId))
            profile.CandidateTypeId = r.CandidateTypeId;

        profile.TargetEntityId  = r.TargetEntityId;

        var needsSponsor = ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider);
        var needsBirthCertificate = ProfileValidatorUtils.RequiresBirthCertificate(profile.CandidateTypeId);
        var needsMarriageCertificate = ProfileValidatorUtils.RequiresMarriageCertificate(profile.CandidateTypeId);
        var requiresNationalAddress = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);

        if (!isLockedProvider)
        {
            profile.QIDExpiry = requiresNationalAddress ? r.QIDExpiry ?? profile.QIDExpiry : null;
        }
        else if (requiresNationalAddress)
        {
            profile.QIDExpiry = profile.QIDExpiry ?? r.QIDExpiry;
        }

        // CV
        var cvResult = await UploadIfNeededAsync(r.CvFile, profile.ResumeAttachmentId, ProfileFileCategories.Cv);
        if (cvResult.IsFailed)  return Result.Fail<Unit>(cvResult.Errors);
        profile.ResumeAttachmentId = cvResult.Value;

        // ID
        var idResult = await UploadIfNeededAsync(r.IdFile, profile.NationalCardId, ProfileFileCategories.NationalId);
        if (idResult.IsFailed) return Result.Fail<Unit>(idResult.Errors);
        profile.NationalCardId = idResult.Value;

        // Birth Certificate
        if (needsBirthCertificate)
        {
            var birthResult = await UploadIfNeededAsync(r.BirthCertificateFile, profile.BirthdayCertificateId, ProfileFileCategories.BirthCertificate);
            if (birthResult.IsFailed) return Result.Fail<Unit>(birthResult.Errors);
            profile.BirthdayCertificateId = birthResult.Value;
        }

        // Marriage Certificate
        if (needsMarriageCertificate)
        {
            var marriageResult = await UploadIfNeededAsync(r.MarriageCertificateFile, profile.MarriageCertificateId, ProfileFileCategories.MarriageCertificate);
            if (marriageResult.IsFailed) return Result.Fail<Unit>(marriageResult.Errors);
            profile.MarriageCertificateId = marriageResult.Value;
        }

        CleanCandidateTypeDependents();
        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Prerequisites, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        void CleanCandidateTypeDependents()
        {
            if (!needsBirthCertificate)
            {
                profile.BirthdayCertificateId = null;
            }

            if (!needsMarriageCertificate)
            {
                profile.MarriageCertificateId = null;
            }

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


