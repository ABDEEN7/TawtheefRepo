using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Commands;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;
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
public class JobInvitationSummaryController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/job-categories")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/departments")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetDepartments()
    {
        var result = await mediator.Send(new GetDepartmentsQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-statuses")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetJobStatusesQuery()
    {
        var result = await mediator.Send(new GetJobStatusesQuery());
        return result.ToActionResult();
    }
    #endregion
    
    #region Retrive Job Invitation Data
    [HttpGet("get-invitations-summary")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetInvitationsSummary([FromQuery] GetJobInvitationSummaryQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("export")]
    [AuthorizePermission(PermissionKeys.Dashboard.Export)]
    public async Task<IActionResult> ExportInvitationsSummary(
        [FromQuery] ExportJobInvitationSummaryQuery query)
    {
        var result = await mediator.Send(query);
        return result.IsFailed
            ? result.ToActionResult()
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }
    
    
    [HttpGet("{jobId:guid}/info")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetInvitationJobInfo(Guid jobId)
    {
        var result = await mediator.Send(new GetJobInvitationSummaryDetailsInfoQuery(jobId));
        return result.ToActionResult();
    }

    [HttpPost("get-invitations-stats")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetInvitationStats([FromBody] GetJobInvitationSummaryDetailsStatsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost("get-invitations-details")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetInvitationRows([FromBody] GetJobInvitationSummaryDetailsRowsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
    [HttpGet("{invitationId:guid}/attachments")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.View)]
    public async Task<IActionResult> GetInvitationAttachments(Guid invitationId)
    {
        var result = await mediator.Send(new GetInvitationAttachmentsQuery(invitationId));
        return result.ToActionResult();
    }

    [HttpPost("{invitationId:guid}/attachments/{attachmentId:guid}/review")]
    [AuthorizePermission(PermissionKeys.JobsInvitations.ManageAttachment)]
    public async Task<IActionResult> ReviewInvitationAttachment(
        Guid invitationId,
        Guid attachmentId,
        [FromBody] ReviewInvitationAttachmentCommand command)
    {
        if (command.InvitationId != invitationId || command.AttachmentId != attachmentId)
            return BadRequest();

        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion
}

