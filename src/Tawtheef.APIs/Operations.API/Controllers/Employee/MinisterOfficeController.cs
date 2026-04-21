using Application.Operation.Features.Employee.MinisterOffice.Commands;
using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using Application.Operation.Features.Employee.MinisterOffice.Queries;
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
public class MinisterOfficeController(IMediator mediator) : ControllerBase
{
    [HttpPost("candidates")]
    [AuthorizePermission(PermissionKeys.MinisterOffice.Manage)]
    public async Task<IActionResult> CreateCandidate(
        [FromBody] CreateMinisterOfficeCandidateRequest request)
    {
        var result = await mediator.Send(new CreateMinisterOfficeCandidateCommand(request));
        return result.ToActionResult();
    }

    [HttpGet("candidates")]
    [AuthorizePermission(PermissionKeys.MinisterOffice.View)]
    public async Task<IActionResult> ListCandidates([FromQuery] GetMinisterOfficeCandidatesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut("candidates/{id:guid}/phone")]
    [AuthorizePermission(PermissionKeys.MinisterOffice.Manage)]
    public async Task<IActionResult> UpdatePhone(
        Guid id,
        [FromBody] UpdateMinisterOfficeCandidatePhoneRequest request)
    {
        var result = await mediator.Send(
            new UpdateMinisterOfficeCandidatePhoneCommand(id, request));
        return result.ToActionResult();
    }

    [HttpPut("candidates/{id:guid}/follow-up-status")]
    [AuthorizePermission(PermissionKeys.MinisterOffice.Manage)]
    public async Task<IActionResult> UpdateFollowUpStatus(
        Guid id,
        [FromBody] UpdateFollowUpStatusRequest request)
    {
        var result = await mediator.Send(
            new UpdateMinisterOfficeCandidateFollowUpStatusCommand(id, request.IsActive));
        return result.ToActionResult();
    }

    [HttpGet("candidates/{id:guid}/invitations")]
    [AuthorizePermission(PermissionKeys.MinisterOffice.View)]
    public async Task<IActionResult> GetInvitations(Guid id)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(
            new GetMinisterOfficeCandidateInvitationsQuery(id, language));
        return result.ToActionResult();
    }

    [HttpGet("candidates/{id:guid}/audit-log")]
    [AuthorizePermission(PermissionKeys.MinisterOffice.View)]
    public async Task<IActionResult> GetAuditLog(
        Guid id,
        [FromQuery] GetMinisterOfficeCandidateAuditLogQuery query)
    {
        var result = await mediator.Send(query with { CandidateId = id });
        return result.ToActionResult();
    }

    #region Lookups

    [HttpGet("lookups/genders")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetGenders()
    {
        var result = await mediator.Send(new GetGendersQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/target-entities")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetTargetEntities()
    {
        var result = await mediator.Send(new GetTargetEntitiesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/candidate-types")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetCandidateTypes()
    {
        var result = await mediator.Send(new GetCandidateTypesQuery());
        return result.ToActionResult();
    }
    #endregion
}

public record UpdateFollowUpStatusRequest(bool IsActive);
