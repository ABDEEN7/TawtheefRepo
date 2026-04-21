using Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class JobCandidatesController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/candidate-types")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetCandidateTypes()
    {
        var result = await mediator.Send(new GetCandidateTypesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/nationalities")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetNationalitiesTypes()
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery() with { Language = language });
        return result.ToActionResult();
    }
    #endregion

    #region Job Candidates
    [HttpGet("filters")]
    [AuthorizePermission(PermissionKeys.Jobs.View)]
    public async Task<IActionResult> GetFilterSettings([FromQuery] GetJobCandidatesFilterSettingsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost("filters")]
    [AuthorizePermission(PermissionKeys.Jobs.View,PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> SaveFilterSettings([FromBody] SaveJobCandidatesFilterSettingsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("category-settings")]
    [AuthorizePermission(PermissionKeys.JobCategoryCandidateSettings.View)]
    public async Task<IActionResult> GetCategorySettings()
    {
        var result = await mediator.Send(new GetJobCategoryCandidateSettingsQuery());
        return result.ToActionResult();
    }

    [HttpPost("category-settings")]
    [AuthorizePermission(PermissionKeys.JobCategoryCandidateSettings.Manage)]
    public async Task<IActionResult> SaveCategorySettings([FromBody] SaveJobCategoryCandidateSettingsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("search")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> Search([FromBody] GetJobCandidatesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("profile")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetCandidateProfile([FromQuery] GetJobCandidateProfileQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost("export")]
    [AuthorizePermission(PermissionKeys.Jobs.View,PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> Export([FromBody] ExportJobCandidatesQuery query)
    {
        var result = await mediator.Send(query);
        if (result.IsFailed)
        {
            return result.ToActionResult();
        }

        var payload = result.Value;
        return File(payload.Content, payload.ContentType, payload.FileName);
    }

    [HttpPost("send-invitations")]
    [AuthorizePermission(PermissionKeys.Jobs.SendInvitation)]
    public async Task<IActionResult> SendInvitations([FromBody] SendJobCandidateInvitationsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion
}

