using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Constants;
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

    [HttpPost("education")]
    public async Task<IActionResult> SaveEducation([FromForm] SaveProfileEducationRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileEducationCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
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

    [HttpPost("skills")]
    public async Task<IActionResult> SaveSkills([FromForm] SaveProfileSkillsRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileSkillsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("attachments")]
    public async Task<IActionResult> SaveAttachments([FromForm] SaveProfileAttachmentsRequest request, CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var cmd = new SaveProfileAttachmentsCommand(UserId.Value, request);
        var result = await mediator.Send(cmd, ct);
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

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(new GetFullUserProfileQuery(UserId.Value), ct); // create DTO with all fields needed by wizard
        return result.ToActionResult();
    }
    #region Lookups
    [HttpGet("lookups/skill-search")]
    public async Task<IActionResult> Search([FromQuery] SearchSkillsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
    [HttpGet("lookups/candidate-types")]
    public async Task<IActionResult> GetCandidateTypes()
    {
        var result = await mediator.Send(new GetCandidateTypesQuery());
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
        var result = await mediator.Send(new GetCountriesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/degrees")]
    public async Task<IActionResult> GetDegrees()
    {
        var result = await mediator.Send(new GetDegreesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/universities")]
    public async Task<IActionResult> GetUniversities()
    {
        var result = await mediator.Send(new GetUniversitiesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/majors")]
    public async Task<IActionResult> GetMajors()
    {
        var result = await mediator.Send(new GetMajorsQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/study-types")]
    public async Task<IActionResult> GetStudyTypes()
    {
        var result = await mediator.Send(new GetStudyTypesQuery());
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

    #endregion
}
