using FluentResults;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Validations;

public interface IProfileStepValidationService
{
    Result ValidatePrerequisites(UserProfile profile, Guid candidateTypeId);

    Result ValidatePersonal(UserProfile profile,
        (string? SponsorEmployerName, string? SponsorEmployerNumber, string? SponsorCardFileName, object? SponsorCard)
            request);

    Result ValidateContact(UserProfile profile, string? address,
        (int Zone, int Street, int Building, int Unit, string? NationalAddressFileName)? nationalAddress);
    Result ValidateEducation(UserProfile profile);
    Result ValidateExperience(UserProfile profile);
    Result ValidateAchievements(UserProfile profile);
    Result ValidateSkills(UserProfile profile);
    Result ValidateLanguages(UserProfile profile);
    Result ValidateAttachments(UserProfile profile);
}
