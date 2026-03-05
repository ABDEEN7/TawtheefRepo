using FluentResults;
using Tawtheef.Application.Common.Validations;
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

    public Result ValidatePrerequisites(UserProfile profile, Guid candidateTypeId)
    {
        var candidateTypeIntegrity = EnsureCandidateTypeIntegrity(profile.CandidateTypeId, candidateTypeId);
        if (candidateTypeIntegrity.IsFailed)
            return candidateTypeIntegrity;

        return Result.Ok();
    }

    public Result ValidatePersonal(UserProfile profile, 
        (string? SponsorEmployerName, string? SponsorEmployerNumber, string? SponsorCardFileName, object? SponsorCard) request)
    {
        var previousSteps = EnsurePreviousStepsCompleted(profile, ProfileStep.Personal);
        if (previousSteps.IsFailed)
            return previousSteps;

        var requiresSponsor = ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider);
        var hasSponsorPayload = !string.IsNullOrWhiteSpace(request.SponsorEmployerName) ||
                                !string.IsNullOrWhiteSpace(request.SponsorEmployerNumber) ||
                                request.SponsorCard is not null || request.SponsorCardFileName is not null;
        var hasSponsorInput = hasSponsorPayload || profile.SponsorProfileId is not null;

        if (requiresSponsor && !hasSponsorInput)
            return Result.Fail(ErrorsCodes.SponsorCardRequired);

        return Result.Ok();
    }

    public Result ValidateContact(UserProfile profile, string? address,
        (int Zone, int Street, int Building, int Unit, string? NationalAddressFileName)? nationalAddress)
    {
        var previousSteps = EnsurePreviousStepsCompleted(profile, ProfileStep.Contact);
        if (previousSteps.IsFailed)
            return previousSteps;

        var requiresNationalAddress = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);
        var hasNationalAddress = nationalAddress is not null || profile.ResidenceAddress is not null;

        if (!requiresNationalAddress && (nationalAddress is not null || profile.ResidenceAddress is not null))
            return Result.Fail(ErrorsCodes.NationalAddressNotAllowed);

        if (requiresNationalAddress && !hasNationalAddress)
            return Result.Fail(ErrorsCodes.NationalAddressRequired);
        
        if (!requiresNationalAddress && string.IsNullOrWhiteSpace(address))
            return Result.Fail(ErrorsCodes.AddressRequired);

        if (requiresNationalAddress)
        {
            if (nationalAddress is not null)
            {
                if (nationalAddress.Value.Zone <= 0 || nationalAddress.Value.Street <= 0 || nationalAddress.Value.Building <= 0 || nationalAddress.Value.Unit < 0)
                    return Result.Fail(ErrorsCodes.InvalidNationalAddress);
                if(string.IsNullOrEmpty(nationalAddress.Value.NationalAddressFileName) && profile.ResidenceAddress!.CertificateId == Guid.Empty)
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
            var validation = GetStepValidationResult(step, profile);
            if (validation.IsFailed)
            {
                var missingFields = string.Join(",", validation.Errors.Select(e => e.Message));
                return Result.Fail($"{ErrorsCodes.PreviousProfileStepIncomplete}:{step}:{missingFields}");
            }
        }

        return Result.Ok();
    }

    private static Result EnsureCandidateTypeIntegrity(Guid? existingCandidateTypeId, Guid incomingCandidateTypeId)
    {
        if (existingCandidateTypeId != Guid.Empty && existingCandidateTypeId != incomingCandidateTypeId)
            return Result.Fail(ErrorsCodes.CandidateTypeChangeNotAllowed);

        return Result.Ok();
    }

    private static Result GetStepValidationResult(ProfileStep step, UserProfile profile) => step switch
    {
        ProfileStep.Prerequisites => ValidateStepPrerequisites(profile),
        ProfileStep.Personal => ValidateStepPersonal(profile),
        ProfileStep.Contact => ValidateStepContact(profile),
        ProfileStep.Education => profile.Qualifications is { Count: > 0 } ? Result.Ok() : Result.Fail("atLeastOne"),
        ProfileStep.Experience => ValidateStepExperience(profile),
        ProfileStep.Achievements => ValidateStepAchievements(profile),
        ProfileStep.Skills => ValidateStepSkills(profile),
        ProfileStep.Languages => profile.Languages is { Count: > 0 } ? Result.Ok() : Result.Fail("atLeastOne"),
        ProfileStep.Attachments => ValidateStepAttachments(profile),
        _ => Result.Ok()
    };

    private static Result ValidateStepExperience(UserProfile profile)
    {
        var missing = new List<string>();
        return missing.Count > 0 ? Result.Fail(missing) : Result.Ok();
    }

    private static Result ValidateStepAchievements(UserProfile profile)
    {
        var missing = new List<string>();
        // Check if user has at least one achievement or course if required
        // Implementation might depend on business rules, for now checking if list exist
        if (profile.Achievements is null) missing.Add("listMissing");
        
        return missing.Count > 0 ? Result.Fail(missing) : Result.Ok();
    }

    private static Result ValidateStepSkills(UserProfile profile)
    {
        var missing = new List<string>();
        return missing.Count > 0 ? Result.Fail(missing) : Result.Ok();
    }

    private static Result ValidateStepAttachments(UserProfile profile)
    {
        var missing = new List<string>();
        return missing.Count > 0 ? Result.Fail(missing) : Result.Ok();
    }

    private static Result ValidateStepPrerequisites(UserProfile profile)
    {
        var missing = new List<string>();
        if (profile.CandidateTypeId is null || profile.CandidateTypeId == Guid.Empty) missing.Add("candidateType");
        if (profile.TargetEntityId is null || profile.TargetEntityId == Guid.Empty) missing.Add("targetEntity");
        if (profile.ResumeAttachmentId is null) missing.Add("cv");
        if (profile.NationalCardId is null) missing.Add("id");

        var requiresResidencyExpiry = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);
        if (requiresResidencyExpiry && profile.QIDExpiry is null)
            missing.Add("qidExpiry");

        if (ProfileValidatorUtils.RequiresBirthCertificate(profile.CandidateTypeId) && profile.BirthdayCertificateId is null)
            missing.Add("birthCertificate");

        if (ProfileValidatorUtils.RequiresMarriageCertificate(profile.CandidateTypeId) && profile.MarriageCertificateId is null)
            missing.Add("marriageCertificate");

        return missing.Count > 0 ? Result.Fail(missing) : Result.Ok();
    }

    private static Result ValidateStepPersonal(UserProfile profile)
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(profile.NationalNumber)) missing.Add("qid");
        if (profile.BirthDate is null) missing.Add("dob");

        if (ProfileValidatorUtils.IsResidentQatar(profile.CandidateTypeId, profile.Provider))
        {
            if (profile.QIDExpiry is null)
                missing.Add("qidExpiry");

            if (!ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider) && profile.SponsorProfileId is not null)
                missing.Add("sponsorType"); // or some other related field
        }

        if (profile.NationalityId is null) missing.Add("nationality");
        if (profile.GenderId is null) missing.Add("gender");
        if (profile.ReligionId is null) missing.Add("religion");
        if (profile.MaritalStatusId is null) missing.Add("marital");

        if (profile.ChildrenCount < 0) missing.Add("childrenCount");

        return missing.Count > 0 ? Result.Fail(missing) : Result.Ok();
    }

    private static Result ValidateStepContact(UserProfile profile)
    {
        var missing = new List<string>();
        if (profile.ResidenceCountryId is null) missing.Add("country");
        if (profile.InterviewLocationId is null) missing.Add("interviewPlace");

        var requiresNationalAddress = ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider);
        if (requiresNationalAddress)
        {
            if (profile.ResidenceAddress is null)
            {
                missing.Add("nationalAddress");
            }
            else
            {
                if (profile.ResidenceAddress.ZoneNo <= 0) missing.Add("naZone");
                if (profile.ResidenceAddress.StreetNo <= 0) missing.Add("naStreet");
                if (profile.ResidenceAddress.BuildingNo <= 0) missing.Add("naBuilding");
                if (profile.ResidenceAddress.UnitNo < 0) missing.Add("naUnit");
                if (profile.ResidenceAddress.CertificateId == Guid.Empty) missing.Add("naFile");
            }
        }
        else
        {
            if (ProfileValidatorUtils.RequiresOffice(profile.CandidateTypeId, profile.Provider) && profile.OfficeId is null)
                missing.Add("office");

            if (string.IsNullOrWhiteSpace(profile.Address))
                missing.Add("address");
        }

        return missing.Count > 0 ? Result.Fail(missing) : Result.Ok();
    }
}
