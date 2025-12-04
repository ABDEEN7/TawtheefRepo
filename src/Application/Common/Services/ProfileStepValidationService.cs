using FluentResults;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Services;

public interface IProfileStepValidationService
{
    Result ValidatePrerequisites(UserProfile profile, SaveProfilePrereqRequest request);
    Result ValidatePersonal(UserProfile profile, SaveProfilePersonalRequest request);
    Result ValidateContact(UserProfile profile, SaveProfileContactRequest request);
    Result ValidateEducation(UserProfile profile);
    Result ValidateExperience(UserProfile profile);
    Result ValidateSkillsAndLanguages(UserProfile profile);
    Result ValidateAttachments(UserProfile profile);
}

public sealed class ProfileStepValidationService : IProfileStepValidationService
{
    private enum ProfileStep
    {
        Prerequisites = 0,
        Personal = 1,
        Contact = 2,
        Education = 3,
        Experience = 4,
        SkillsLanguages = 5,
        Attachments = 6
    }

    private static readonly ProfileStep[] StepOrder =
    [
        ProfileStep.Prerequisites,
        ProfileStep.Personal,
        ProfileStep.Contact,
        ProfileStep.Education,
        ProfileStep.Experience,
        ProfileStep.SkillsLanguages,
        ProfileStep.Attachments
    ];

    public Result ValidatePrerequisites(UserProfile profile, SaveProfilePrereqRequest request)
    {
        return EnsureCandidateTypeIntegrity(profile.CandidateTypeId, request.CandidateTypeId);
    }

    public Result ValidatePersonal(UserProfile profile, SaveProfilePersonalRequest request)
    {
        var previousSteps = EnsurePreviousStepsCompleted(profile, ProfileStep.Personal);
        if (previousSteps.IsFailed)
            return previousSteps;

        var allowsSponsor = AllowsSponsor(profile.CandidateTypeId);
        var requiresSponsor = RequiresSponsor(profile.CandidateTypeId);
        var hasSponsorInput = HasSponsorPayload(request) || profile.SponsorProfileId is not null;

        if (!allowsSponsor && hasSponsorInput)
            return Result.Fail(ErrorsCodes.SponsorNotAllowed);

        if (requiresSponsor && !hasSponsorInput)
            return Result.Fail(ErrorsCodes.SponsorCardRequired);

        return Result.Ok();
    }

    public Result ValidateContact(UserProfile profile, SaveProfileContactRequest request)
    {
        var previousSteps = EnsurePreviousStepsCompleted(profile, ProfileStep.Contact);
        if (previousSteps.IsFailed)
            return previousSteps;

        var requiresNationalAddress = RequiresNationalAddress(profile.CandidateTypeId);
        var hasNationalAddress = request.NationalAddress is not null || profile.ResidenceAddress is not null;

        if (!requiresNationalAddress && (request.NationalAddress is not null || profile.ResidenceAddress is not null))
            return Result.Fail(ErrorsCodes.NationalAddressNotAllowed);

        if (requiresNationalAddress && !hasNationalAddress)
            return Result.Fail(ErrorsCodes.NationalAddressRequired);

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

    public Result ValidateSkillsAndLanguages(UserProfile profile)
    {
        return EnsurePreviousStepsCompleted(profile, ProfileStep.SkillsLanguages);
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
        ProfileStep.Experience => profile.Experiences is { Count: > 0 } || profile.TrainingCourses is { Count: > 0 },
        ProfileStep.SkillsLanguages => profile.Skills is { Count: > 0 } && profile.Languages is { Count: > 0 },
        ProfileStep.Attachments => profile.AdditionalAttachments is { Count: > 0 },
        _ => false
    };

    private static bool IsPrerequisitesComplete(UserProfile profile)
    {
        if (profile.CandidateTypeId == Guid.Empty || profile.TargetEntityId == Guid.Empty)
            return false;

        if (profile.ResumeAttachmentId is null || profile.NationalCardId is null)
            return false;

        if (RequiresBirthCertificate(profile.CandidateTypeId) && profile.BirthdayCertificateId is null)
            return false;

        if (RequiresMarriageCertificate(profile.CandidateTypeId) && profile.MarriageCertificateId is null)
            return false;

        if (RequiresOffice(profile.CandidateTypeId) && profile.OfficeId is null)
            return false;

        return true;
    }

    private static bool IsPersonalComplete(UserProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.NationalNumber) || profile.BirthDate is null || profile.QIDExpiry is null)
            return false;

        if (profile.NationalityId is null || profile.GenderId is null || profile.ReligionId is null || profile.MaritalStatusId is null)
            return false;

        if (profile.ChildrenCount < 0)
            return false;

        if (profile.HasDisability && string.IsNullOrWhiteSpace(profile.DisabilityDetails))
            return false;

        if (RequiresSponsor(profile.CandidateTypeId) && profile.SponsorProfileId is null)
            return false;

        if (!AllowsSponsor(profile.CandidateTypeId) && profile.SponsorProfileId is not null)
            return false;

        return true;
    }

    private static bool IsContactComplete(UserProfile profile)
    {
        if (profile.ResidenceCountryId is null || profile.InterviewLocationId is null)
            return false;

        var requiresNationalAddress = RequiresNationalAddress(profile.CandidateTypeId);
        if (requiresNationalAddress)
        {
            if (profile.ResidenceAddress is null || profile.ResidenceAddressCertificateId is null)
                return false;

            return profile.ResidenceAddress.ZoneNo > 0 &&
                   profile.ResidenceAddress.StreetNo > 0 &&
                   profile.ResidenceAddress.BuildingNo > 0 &&
                   profile.ResidenceAddress.UnitNo >= 0;
        }

        return !string.IsNullOrWhiteSpace(profile.Address);
    }

    private static bool AllowsSponsor(Guid candidateTypeId) => candidateTypeId == CandidateTypeIds.ResidentQatar;
    private static bool RequiresSponsor(Guid candidateTypeId) => candidateTypeId == CandidateTypeIds.ResidentQatar;
    private static bool RequiresNationalAddress(Guid candidateTypeId) => candidateTypeId != CandidateTypeIds.NonQatari && candidateTypeId != CandidateTypeIds.GCC;
    private static bool RequiresOffice(Guid candidateTypeId) => candidateTypeId == CandidateTypeIds.NonQatari || candidateTypeId == CandidateTypeIds.GCC;
    private static bool RequiresBirthCertificate(Guid candidateTypeId) => candidateTypeId == CandidateTypeIds.SonOfQatariMother;
    private static bool RequiresMarriageCertificate(Guid candidateTypeId) => candidateTypeId == CandidateTypeIds.WifeOfQatari;
    private static bool HasSponsorPayload(SaveProfilePersonalRequest request) =>
        !string.IsNullOrWhiteSpace(request.SponsorEmployerName) ||
        !string.IsNullOrWhiteSpace(request.SponsorEmployerNumber) ||
        request.SponsorCard is not null ||
        request.SponsorCardFileName is not null;
}
