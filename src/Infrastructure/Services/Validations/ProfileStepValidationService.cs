using FluentResults;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Utils;

namespace Tawtheef.Infrastructure.Services.Validations;

public sealed class ProfileStepValidationService : IProfileStepValidationService
{
    private enum ProfileStep
    {
        Prerequisites = 0,
        Personal = 1,
        Contact = 2,
        Education = 3,
        Experience = 4,
        Achievements = 5,
        Skills = 6,
        Languages = 7,
        Attachments = 8
    }

    private static readonly ProfileStep[] StepOrder =
    [
        ProfileStep.Prerequisites,
        ProfileStep.Personal,
        ProfileStep.Contact,
        ProfileStep.Education,
        ProfileStep.Experience,
        ProfileStep.Achievements,
        ProfileStep.Skills,
        ProfileStep.Languages,
        ProfileStep.Attachments
    ];

    public Result ValidatePrerequisites(UserProfile profile, SaveProfilePrereqRequest request)
    {
        var candidateTypeIntegrity = EnsureCandidateTypeIntegrity(profile.CandidateTypeId, request.CandidateTypeId);
        if (candidateTypeIntegrity.IsFailed)
            return candidateTypeIntegrity;

        return Result.Ok();
    }

    public Result ValidatePersonal(UserProfile profile, SaveProfilePersonalRequest request)
    {
        var previousSteps = EnsurePreviousStepsCompleted(profile, ProfileStep.Personal);
        if (previousSteps.IsFailed)
            return previousSteps;

        var requiresSponsor = ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider);
        var hasSponsorInput = HasSponsorPayload(request) || profile.SponsorProfileId is not null;

        if (requiresSponsor && !hasSponsorInput)
            return Result.Fail(ErrorsCodes.SponsorCardRequired);

        return Result.Ok();
    }

    public Result ValidateContact(UserProfile profile, SaveProfileContactRequest request)
    {
        var previousSteps = EnsurePreviousStepsCompleted(profile, ProfileStep.Contact);
        if (previousSteps.IsFailed)
            return previousSteps;

        var requiresNationalAddress = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);
        var hasNationalAddress = request.NationalAddress is not null || profile.ResidenceAddress is not null;

        if (!requiresNationalAddress && (request.NationalAddress is not null || profile.ResidenceAddress is not null))
            return Result.Fail(ErrorsCodes.NationalAddressNotAllowed);

        if (requiresNationalAddress && !hasNationalAddress)
            return Result.Fail(ErrorsCodes.NationalAddressRequired);
        
        if (!requiresNationalAddress && string.IsNullOrWhiteSpace(request.Address))
            return Result.Fail(ErrorsCodes.AddressRequired);

        if (requiresNationalAddress)
        {
            if (request.NationalAddress is not null)
            {
                if (request.NationalAddress.Zone <= 0 || request.NationalAddress.Street <= 0 || request.NationalAddress.Building <= 0 || request.NationalAddress.Unit < 0)
                    return Result.Fail(ErrorsCodes.InvalidNationalAddress);
                if(string.IsNullOrEmpty(request.NationalAddress.NationalAddressFileName) && profile.ResidenceAddress!.CertificateId == Guid.Empty)
                    return Result.Fail(ErrorsCodes.NationalAddressCertificateRequired);
            }
        }
        
        return Result.Ok();
    }

    public Result ValidateEducation(UserProfile profile)
    {
        return EnsurePreviousStepsCompleted(profile, ProfileStep.Education);
    }

    public Result ValidateExperience(UserProfile profile)
    {
        return EnsurePreviousStepsCompleted(profile, ProfileStep.Experience);
    }

    public Result ValidateAchievements(UserProfile profile)
    {
        return EnsurePreviousStepsCompleted(profile, ProfileStep.Achievements);
    }

    public Result ValidateSkills(UserProfile profile)
    {
        return EnsurePreviousStepsCompleted(profile, ProfileStep.Skills);
    }

    public Result ValidateLanguages(UserProfile profile)
    {
        return EnsurePreviousStepsCompleted(profile, ProfileStep.Languages);
    }

    public Result ValidateAttachments(UserProfile profile)
    {
        return EnsurePreviousStepsCompleted(profile, ProfileStep.Attachments);
    }

    private static Result EnsurePreviousStepsCompleted(UserProfile profile, ProfileStep currentStep)
    {
        var currentIndex = Array.IndexOf(StepOrder, currentStep);
        for (var i = 0; i < currentIndex; i++)
        {
            var step = StepOrder[i];
            if (!IsStepComplete(step, profile))
            {
                return Result.Fail($"{ErrorsCodes.PreviousProfileStepIncomplete}:{step}");
            }
        }

        return Result.Ok();
    }

    private static Result EnsureCandidateTypeIntegrity(Guid existingCandidateTypeId, Guid incomingCandidateTypeId)
    {
        if (existingCandidateTypeId != Guid.Empty && existingCandidateTypeId != incomingCandidateTypeId)
            return Result.Fail(ErrorsCodes.CandidateTypeChangeNotAllowed);

        return Result.Ok();
    }

    private static bool IsStepComplete(ProfileStep step, UserProfile profile) => step switch
    {
        ProfileStep.Prerequisites => IsPrerequisitesComplete(profile),
        ProfileStep.Personal => IsPersonalComplete(profile),
        ProfileStep.Contact => IsContactComplete(profile),
        ProfileStep.Education => profile.Qualifications is { Count: > 0 },
        ProfileStep.Experience => true,
        ProfileStep.Achievements => true,
        ProfileStep.Skills => true,
        ProfileStep.Languages => profile.Languages is { Count: > 0 },
        ProfileStep.Attachments => true,
        _ => false
    };

    private static bool IsPrerequisitesComplete(UserProfile profile)
    {
        if (profile.CandidateTypeId == Guid.Empty || profile.TargetEntityId == Guid.Empty)
            return false;

        if (profile.ResumeAttachmentId is null || profile.NationalCardId is null)
            return false;

        var requiresResidencyExpiry = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);
        if (requiresResidencyExpiry && profile.QIDExpiry is null)
            return false;

        if (ProfileValidatorUtils.RequiresBirthCertificate(profile.CandidateTypeId) && profile.BirthdayCertificateId is null)
            return false;

        if (ProfileValidatorUtils.RequiresMarriageCertificate(profile.CandidateTypeId) && profile.MarriageCertificateId is null)
            return false;

        return true;
    }

    private static bool IsPersonalComplete(UserProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.NationalNumber) || profile.BirthDate is null)
            return false;

        if (ProfileValidatorUtils.IsResidentQatar(profile.CandidateTypeId, profile.Provider))
        {
            if(profile.QIDExpiry is null)
                return false;

            if (!ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider) && profile.SponsorProfileId is not null)
                return false;
        }

        if (profile.NationalityId is null || profile.GenderId is null ||
            profile.ReligionId is null || profile.MaritalStatusId is null)
            return false;

        if (profile.ChildrenCount < 0)
            return false;

        if (profile.HasDisability && string.IsNullOrWhiteSpace(profile.DisabilityDetails))
            return false;

        return true;
    }

    private static bool IsContactComplete(UserProfile profile)
    {
        if (profile.ResidenceCountryId is null || profile.InterviewLocationId is null)
            return false;

        var requiresNationalAddress = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);
        if (requiresNationalAddress)
        {
            if (profile.ResidenceAddress is null)
                return false;

            return profile.ResidenceAddress.ZoneNo > 0 &&
                   profile.ResidenceAddress.StreetNo > 0 &&
                   profile.ResidenceAddress.BuildingNo > 0 &&
                   profile.ResidenceAddress.UnitNo >= 0 &&
                   profile.ResidenceAddress.CertificateId != Guid.Empty;
        }

        if (ProfileValidatorUtils.RequiresOffice(profile.CandidateTypeId, profile.Provider) && profile.OfficeId is null)
            return false;

        return !string.IsNullOrWhiteSpace(profile.Address);
    }
    
    private static bool HasSponsorPayload(SaveProfilePersonalRequest request) =>
        !string.IsNullOrWhiteSpace(request.SponsorEmployerName) ||
        !string.IsNullOrWhiteSpace(request.SponsorEmployerNumber) ||
        request.SponsorCard is not null ||
        request.SponsorCardFileName is not null;
}
