using System.Security.Claims;
using Application.Recruitment.Features.Dashboard.Commands;
using Application.Recruitment.Features.Dashboard.Queries;
using Application.Recruitment.Features.JobDetails.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers.Applicant;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DashboardController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };

    #region Lookups
    [HttpGet("lookups/invitation-statuses")]
    public async Task<IActionResult> GetInvitationStatuses()
    {
        var result = await mediator.Send(new GetInvitationStatusesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-categories")]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/department")]
    public async Task<IActionResult> GetDepartments()
    {
        var result = await mediator.Send(new GetDepartmentsQuery());
        return result.ToActionResult();
    }
    #endregion

    #region Retrive Dashboard Data
    [HttpGet("get-candidate-invitations")]
    public async Task<IActionResult> GetCandidateInvitations([FromQuery] GetCandidateInvitationsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("candidate-invitations/{invitationId:guid}")]
    public async Task<IActionResult> GetCandidateInvitationDetails(Guid invitationId)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetCandidateInvitationDetailsQuery(invitationId, UserId.Value));
        return result.ToActionResult();
    }

    [HttpGet("candidate-invitations/{invitationId:guid}/job-details")]
    public async Task<IActionResult> GetCandidateJobDetails(Guid invitationId)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetCandidateJobDetailsQuery(invitationId, UserId.Value));
        return result.ToActionResult();
    }

    [HttpPost("candidate-invitations/{invitationId:guid}/apply")]
    public async Task<IActionResult> ApplyCandidateInvitation(Guid invitationId)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new ApplyCandidateInvitationCommand(UserId.Value, invitationId));
        return result.ToActionResult();
    }
    
    [HttpPost("candidate-invitations/{invitationId:guid}/read")]
    public async Task<IActionResult> ChangeStatusCandidateInvitationRead(
        Guid invitationId)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new ChangeStatusCandidateInvitationReadCommand(
            UserId.Value,
            invitationId));
        return result.ToActionResult();
    }
    
    [HttpPost("candidate-invitations/{invitationId:guid}/reject")]
    public async Task<IActionResult> ChangeStatusCandidateInvitationReject(
        Guid invitationId)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new ChangeStatusCandidateInvitationRejectedCommand(
            UserId.Value,
            invitationId));
        return result.ToActionResult();
    }

    [HttpGet("candidate-invitation-statistics")]
    public async Task<IActionResult> GetCandidateInvitationStatistics()
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetCandidateInvitationStatisticsQuery(UserId.Value));
        return result.ToActionResult();
    }
    [HttpPost("candidate-invitations/{invitationId}/attachments/{jobRequiredAttachmentId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCandidateInvitationAttachment(
        Guid invitationId,
        Guid jobRequiredAttachmentId,
        IFormFile file)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new UploadInvitationAttachmentCommand(UserId.Value, invitationId, jobRequiredAttachmentId, file));
        
        return result.ToActionResult();
    }

    [HttpDelete("candidate-invitations/{invitationId}/attachments/{attachmentId}")]
    public async Task<IActionResult> DeleteCandidateInvitationAttachment(
        Guid invitationId,
        Guid attachmentId)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new DeleteInvitationAttachmentCommand(UserId.Value, invitationId, attachmentId));
        return result.ToActionResult();
    }

    #endregion
}

