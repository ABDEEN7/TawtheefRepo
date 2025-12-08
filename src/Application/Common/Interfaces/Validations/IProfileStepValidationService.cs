using FluentResults;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Services;

public interface IProfileStepValidationService
{
    Result ValidatePrerequisites(UserProfile profile, SaveProfilePrereqRequest request);
    Result ValidatePersonal(UserProfile profile, SaveProfilePersonalRequest request);
    Result ValidateContact(UserProfile profile, SaveProfileContactRequest request);
    Result ValidateEducation(UserProfile profile);
    Result ValidateExperience(UserProfile profile);
    Result ValidateAchievements(UserProfile profile);
    Result ValidateSkills(UserProfile profile);
    Result ValidateLanguages(UserProfile profile);
    Result ValidateAttachments(UserProfile profile);
}
