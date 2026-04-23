using System.Security.Claims;
using Application.Recruitment.Features.Authenticator.DTOs;
using Application.Recruitment.Features.Profile.Command;
using Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;
using Application.Recruitment.Features.Profile.Command.DeleteOperation;
using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Command.SaveOperation;
using Application.Recruitment.Features.Profile.DTOs;
using Application.Recruitment.Features.Profile.DTOs.ReviseOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
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

    [HttpGet("training")]
    public Task<IActionResult> GetTraining(CancellationToken ct) => GetProfileStatus(ct, ProfileSection.TrainingCourses);

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

    #region Profile revision operations

    [HttpPost("revisions/prereq")]
    public async Task<IActionResult> RevisePrereq([FromForm] SaveProfilePrereqRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfilePrereqCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
    [HttpPost("revisions/prereq/attachment")]
    public async Task<IActionResult> RevisePrereq([FromForm] ReviseProfilePrereqAttachmentRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfilePrereqAttachmentsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/personal")]
    public async Task<IActionResult> RevisePersonal([FromForm] SaveProfilePersonalRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfilePersonalCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
    [HttpPost("revisions/personal/attachment")]
    public async Task<IActionResult> RevisePersonalAttachment([FromForm] ReviseProfilePersonalAttachmentRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfilePersonalAttachmentsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/contact")]
    public async Task<IActionResult> ReviseContact([FromForm] SaveProfileContactRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileContactCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
    [HttpPost("revisions/contact/attachment")]
    public async Task<IActionResult> ReviseContact([FromForm] ReviseProfileContactAttachmentRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileContactAttachmentsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/education")]
    public async Task<IActionResult> ReviseEducation([FromForm] SaveProfileEducationRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileEducationCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("revisions/education/{degreeId:guid}/delete")]
    public async Task<IActionResult> ReviseDeleteEducation([FromRoute] Guid degreeId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new ReviseProfileEducationDeleteCommand(UserId.Value, degreeId), ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/experience")]
    public async Task<IActionResult> ReviseExperience([FromForm] SaveProfileExperienceRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileExperienceCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("revisions/experience/{experienceId:guid}/delete")]
    public async Task<IActionResult> ReviseDeleteExperience([FromRoute] Guid experienceId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new ReviseProfileExperienceDeleteCommand(UserId.Value, experienceId), ct);
        return result.ToActionResult();
    }

    [HttpDelete("revisions/training/{trainingId:guid}/delete")]
    public async Task<IActionResult> ReviseDeleteTraining([FromRoute] Guid trainingId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new ReviseProfileTrainingDeleteCommand(UserId.Value, trainingId), ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/achievements")]
    public async Task<IActionResult> ReviseAchievements([FromForm] SaveProfileAchievementRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileAchievementCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("revisions/achievement/{achievementId:guid}/delete")]
    public async Task<IActionResult> ReviseDeleteAchievement([FromRoute] Guid achievementId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new ReviseProfileAchievementDeleteCommand(UserId.Value, achievementId), ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/skills")]
    public async Task<IActionResult> ReviseSkills([FromBody] SaveProfileSkillsRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileSkillsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("revisions/skill/{skillId:guid}/delete")]
    public async Task<IActionResult> ReviseDeleteSkill([FromRoute] Guid skillId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new ReviseProfileSkillDeleteCommand(UserId.Value, skillId), ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/languages")]
    public async Task<IActionResult> ReviseLanguages([FromBody] SaveProfileLanguagesRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileLanguagesCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("revisions/language/{languageId:guid}/delete")]
    public async Task<IActionResult> ReviseDeleteLanguage([FromRoute] Guid languageId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new ReviseProfileLanguageDeleteCommand(UserId.Value, languageId), ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/references")]
    public async Task<IActionResult> ReviseAttachments([FromForm] SaveProfileAttachmentsRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ReviseProfileAttachmentsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpDelete("revisions/references/{attachmentId:guid}/delete")]
    public async Task<IActionResult> ReviseDeleteAttachment([FromRoute] Guid attachmentId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new ReviseProfileAttachmentDeleteCommand(UserId.Value, attachmentId), ct);
        return result.ToActionResult();
    }

    [HttpPost("revisions/submit")]
    public async Task<IActionResult> ResubmitProfile([FromBody] SubmitUserProfileRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new ResubmitUserProfileCommand(UserId.Value, request);
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

    [HttpDelete("education/{degreeId:guid}/delete")]
    public async Task<IActionResult> DeleteEducation([FromRoute] Guid degreeId, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new DeleteProfileEducationCommand(UserId.Value, degreeId), ct);
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

    [HttpDelete("experience/{experienceId:guid}/delete")]
    public async Task<IActionResult> DeleteExperience([FromRoute] Guid experienceId, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new DeleteProfileExperienceCommand(UserId.Value, experienceId), ct);
        return result.ToActionResult();
    }

    [HttpDelete("training/{trainingId:guid}/delete")]
    public async Task<IActionResult> DeleteTraining([FromRoute] Guid trainingId, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new DeleteProfileTrainingCommand(UserId.Value, trainingId), ct);
        return result.ToActionResult();
    }

    [HttpDelete("achievement/{achievementId:guid}/delete")]
    public async Task<IActionResult> DeleteAchievement([FromRoute] Guid achievementId, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new DeleteProfileAchievementCommand(UserId.Value, achievementId), ct);
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

    [HttpDelete("skill/{skillId:guid}/delete")]
    public async Task<IActionResult> DeleteSkill([FromRoute] Guid skillId, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new DeleteProfileSkillCommand(UserId.Value, skillId), ct);
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
    
    [HttpDelete("language/{languageId:guid}/delete")]
    public async Task<IActionResult> DeleteLanguage([FromRoute] Guid languageId, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new DeleteProfileLanguageCommand(UserId.Value, languageId), ct);
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

    [HttpDelete("references/{attachmentId:guid}/delete")]
    public async Task<IActionResult> DeleteAttachment([FromRoute] Guid attachmentId, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new DeleteProfileAttachmentCommand(UserId.Value, attachmentId), ct);
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
    public async Task<IActionResult> CheckProfile([FromBody] CheckProfileMOI request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var query = new GetPersonalInformationByQidQuery(request);
        var result = await mediator.Send(query, ct);
        return result.ToActionResult();
    }
    

    #endregion
   
    #region Lookups
    [HttpGet("lookups/skill-search")]
    public async Task<IActionResult> Search([FromQuery] GetSkillsBasedOnMajorQuery query)
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
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        var result = await mediator.Send(new GetCandidateTypesByProviderQuery(provider, UserId.Value));
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

        if (result.IsSuccess && User.HasClaim("login_provider", nameof(ProviderLoginIds.Google)))
        {
            result.Value.RemoveAll(c => c.Id == CountryIds.Qatar);
        }
        else
        {
            result.Value.RemoveAll(c => c.Id != CountryIds.Qatar);
        }

        return result.ToActionResult();
    }

    [HttpGet("lookups/nationalities")]
    public async Task<IActionResult> Nationalities()
    {
        //get language from header
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery { Language = language });

        if (result.IsSuccess && User.HasClaim("login_provider", nameof(ProviderLoginIds.Google)))
        {
            result.Value.RemoveAll(c => c.Id == CountryIds.Qatar);
        }

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
    public async Task<IActionResult> GetMajors([FromQuery] GetMainMajorsQuery query)
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

