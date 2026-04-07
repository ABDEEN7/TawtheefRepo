using Application.Operation.Features.Employee.MinisterOffice.Commands;
using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using Application.Operation.Features.Employee.MinisterOffice.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
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
}

public record UpdateFollowUpStatusRequest(bool IsActive);
