using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers.Applicant;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfilesController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };
    
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetMyUserProfileSummaryQuery(UserId.Value), ct);
        return result.ToActionResult();
    }

    [HttpGet("basics")]
    public Task<IActionResult> GetBasics(CancellationToken ct) => GetProfileStatus(ct);

    [HttpGet("prereq")]
    public Task<IActionResult> GetPrerequisites(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Prerequisites);

    [HttpGet("personal")]
    public Task<IActionResult> GetPersonal(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Personal);

    [HttpGet("contact")]
    public Task<IActionResult> GetContact(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Contact);

    [HttpGet("education")]
    public Task<IActionResult> GetEducation(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Qualifications);

    [HttpGet("experience")]
    public Task<IActionResult> GetExperience(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Experience);

    [HttpGet("achievements")]
    public Task<IActionResult> GetAchievements(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.CertificatesAndAwards);

    [HttpGet("skills")]
    public Task<IActionResult> GetSkills(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Skills);

    [HttpGet("languages")]
    public Task<IActionResult> GetLanguages(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Languages);

    [HttpGet("references")]
    public Task<IActionResult> GetReferences(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.Attachments);

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetMyUserProfileQuery(UserId.Value), ct);
        return result.ToActionResult();
    }

    private async Task<IActionResult> GetProfileStatus(CancellationToken ct, ProfileSection? section = null)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetMyProfileStatusQuery(UserId.Value, section), ct);
        return result.ToActionResult();
    }

    [HttpGet("change-requests")]
    public async Task<IActionResult> GetChangeRequests(CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetMyProfileChangeRequestsQuery(UserId.Value), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Returns corrections ONLY when profile is InCreation (after reviewer Finalize with corrections).
    /// Otherwise returns empty list.
    /// </summary>
    [HttpGet("corrections")]
    public async Task<IActionResult> GetCorrections(CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetMyProfileCorrectionsQuery(UserId.Value), ct);
        return result.ToActionResult();
    }

    #region Profile update operations

    [HttpPost("change-requests/prereq")]
    public async Task<IActionResult> RequestPrereqChange([FromForm] SaveProfilePrereqRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfilePrereqChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/personal")]
    public async Task<IActionResult> RequestPersonalChange([FromForm] SaveProfilePersonalRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfilePersonalChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/contact")]
    public async Task<IActionResult> RequestContactChange([FromForm] SaveProfileContactRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfileContactChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/education")]
    public async Task<IActionResult> RequestEducationChange([FromForm] SaveProfileEducationRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfileEducationChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/experience")]
    public async Task<IActionResult> RequestExperienceChange([FromForm] SaveProfileExperienceRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfileExperienceChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/achievements")]
    public async Task<IActionResult> RequestAchievementsChange([FromForm] SaveProfileAchievementRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfileAchievementChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/skills")]
    public async Task<IActionResult> RequestSkillsChange([FromBody] SaveProfileSkillsRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfileSkillsChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/languages")]
    public async Task<IActionResult> RequestLanguagesChange([FromBody] SaveProfileLanguagesRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfileLanguagesChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("change-requests/references")]
    public async Task<IActionResult> RequestAttachmentsChange([FromForm] SaveProfileAttachmentsRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new RequestProfileAttachmentsChangeCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    #endregion
    
    #region Profile creation operations
    
    [HttpPost("prereq")]
    public async Task<IActionResult> SavePrereq([FromForm] SaveProfilePrereqRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfilePrereqCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("personal")]
    public async Task<IActionResult> SavePersonal([FromForm] SaveProfilePersonalRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfilePersonalCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("contact")]
    public async Task<IActionResult> SaveContact([FromForm] SaveProfileContactRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileContactCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("availability")]
    public async Task<IActionResult> SaveAvailability([FromBody] SaveProfileAvailabilityRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileAvailabilityCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("education")]
    public async Task<IActionResult> SaveEducation([FromForm] SaveProfileEducationRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileEducationCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("education/{degreeId}/delete")]
    public async Task<IActionResult> DeleteEducation([FromRoute] DeleteProfileEducationCommand command, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("experience")]
    public async Task<IActionResult> SaveExperience([FromForm] SaveProfileExperienceRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileExperienceCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("achievements")]
    public async Task<IActionResult> SaveAchievements([FromForm] SaveProfileAchievementRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileAchievementCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("experience/{experienceId}/delete")]
    public async Task<IActionResult> DeleteExperience([FromRoute] DeleteProfileExperienceCommand command, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpDelete("training/{trainingId}/delete")]
    public async Task<IActionResult> DeleteTraining([FromRoute] DeleteProfileTrainingCommand command, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpDelete("achievement/{achievementId}/delete")]
    public async Task<IActionResult> DeleteAchievement([FromRoute] DeleteProfileAchievementCommand command, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("skills")]
    public async Task<IActionResult> SaveSkills([FromBody] SaveProfileSkillsRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileSkillsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("skill/{skillId}/delete")]
    public async Task<IActionResult> DeleteSkill([FromRoute] DeleteProfileSkillCommand command, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("languages")]
    public async Task<IActionResult> SaveLanguages([FromBody] SaveProfileLanguagesRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileLanguagesCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
    
    [HttpDelete("language/{languageId}/delete")]
    public async Task<IActionResult> DeleteLanguage([FromRoute] DeleteProfileLanguageCommand command, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("references")]
    public async Task<IActionResult> SaveAttachments([FromForm] SaveProfileAttachmentsRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileAttachmentsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("references/{attachmentId}/delete")]
    public async Task<IActionResult> DeleteAttachment([FromRoute] DeleteProfileAttachmentCommand command, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitProfile([FromBody] SubmitUserProfileRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SubmitUserProfileCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
    
    [HttpPost("check-profile")]
    [EnableRateLimiting(LimitsPolicyKeys.MoiCheckProfilePolicy)]
    public async Task<IActionResult> CheckProfile([FromBody] CheckProfileMOI query, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new GetPersonalInformationByQidQuery(UserId.Value, query);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
    

    #endregion
   
    #region Lookups
    [HttpGet("lookups/skill-search")]
    public async Task<IActionResult> Search([FromQuery] SearchSkillsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
    [HttpGet("lookups/skill-levels")]
    public async Task<IActionResult> Search()
    {
        var result = await mediator.Send(new GetSkillLevelsQuery());
        return result.ToActionResult();
    }
    [HttpGet("lookups/candidate-types")]
    public async Task<IActionResult> GetCandidateTypes([FromQuery] string provider)
    {
        var result = await mediator.Send(new GetCandidateTypesByProviderQuery(provider));
        return result.ToActionResult();
    }

    [HttpGet("lookups/target-entities")]
    public async Task<IActionResult> TargetEntityTypes()
    {
        var result = await mediator.Send(new GetTargetEntitiesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/genders")]
    public async Task<IActionResult> Genders()
    {
        var result = await mediator.Send(new GetGendersQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/religions")]
    public async Task<IActionResult> Religions()
    {
        var result = await mediator.Send(new GetReligionsQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/marital-statuses")]
    public async Task<IActionResult> MaritalStatuses()
    {
        var result = await mediator.Send(new GetMaritalStatusesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/countries")]
    public async Task<IActionResult> Countries()
    {
        //get language from header
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery { Language = language });
        return result.ToActionResult();
    }

    [HttpGet("lookups/degrees")]
    public async Task<IActionResult> GetDegrees()
    {
        var result = await mediator.Send(new GetDegreesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/universities")]
    public async Task<IActionResult> GetUniversities([FromQuery] GetUniversitiesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("lookups/majors")]
    public async Task<IActionResult> GetMajors([FromQuery] GetMajorsQuery query)
    {
        //get language from header
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with {Language = language});
        return result.ToActionResult();
    }

    [HttpGet("lookups/sub-majors")]
    public async Task<IActionResult> GetMajors([FromQuery] GetSubMajorsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/study-types")]
    public async Task<IActionResult> GetStudyTypes()
    {
        var result = await mediator.Send(new GetStudyTypesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/achievement-types")]
    public async Task<IActionResult> GetAchievementTypes()
    {
        var result = await mediator.Send(new GetAchievementTypesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/rating-grades")]
    public async Task<IActionResult> GetRatingGrades()
    {
        var result = await mediator.Send(new GetRatingGradesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/languages")]
    public async Task<IActionResult> GetLanguages()
    {
        var result = await mediator.Send(new GetLanguagesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/language-levels")]
    public async Task<IActionResult> GetLanguageLevels()
    {
        var result = await mediator.Send(new GetLanguageLevelsQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/sponsor-types")]
    public async Task<IActionResult> GetSponsorTypes()
    {
        var result = await mediator.Send(new GetSponsorTypesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/offices")]
    public async Task<IActionResult> GetOffices([FromQuery] GetOfficesQuery query)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language });
        return result.ToActionResult();
    }

    #endregion
}
