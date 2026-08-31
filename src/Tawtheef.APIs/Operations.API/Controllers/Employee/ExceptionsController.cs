using Application.Operation.Features.Employee.Exceptions.Commands;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using Application.Operation.Features.Employee.Exceptions.Queries;
using Application.Operation.Features.Employee.OrganizationStructures.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class ExceptionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Exceptions.View)]
    public async Task<IActionResult> Get(
        [FromQuery] GetInvitationExceptionsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("summary")]
    [AuthorizePermission(PermissionKeys.Exceptions.View)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetInvitationExceptionsSummaryQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{exceptionId:guid}")]
    [AuthorizePermission(PermissionKeys.Exceptions.View)]
    public async Task<IActionResult> GetDetails(Guid exceptionId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetInvitationExceptionDetailsQuery(exceptionId),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{exceptionId:guid}/proof")]
    [AuthorizePermission(PermissionKeys.Exceptions.View)]
    public async Task<IActionResult> GetProof(Guid exceptionId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetInvitationExceptionProofQuery(exceptionId),
            cancellationToken);
        if (result.IsFailed)
            return result.ToActionResult();

        return File(
            result.Value.Stream,
            result.Value.ContentType,
            result.Value.FileName,
            enableRangeProcessing: true);
    }

    [HttpPost("{exceptionId:guid}/cancel")]
    [AuthorizePermission(PermissionKeys.Exceptions.Cancel)]
    public async Task<IActionResult> Cancel(
        Guid exceptionId,
        [FromBody] CancelInvitationExceptionRequestDto request)
    {
        var result = await mediator.Send(new CancelInvitationExceptionCommand(exceptionId, request.Reason));
        return result.ToActionResult();
    }

    [HttpPost("{exceptionId:guid}/send-invitation")]
    [AuthorizePermission(PermissionKeys.Exceptions.SendInvitation)]
    public async Task<IActionResult> SendInvitation(Guid exceptionId)
    {
        var result = await mediator.Send(new SendExceptionalInvitationCommand(exceptionId));
        return result.ToActionResult();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [AuthorizePermission(PermissionKeys.Exceptions.Create)]
    public async Task<IActionResult> Create([FromForm] CreateInvitationExceptionCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("jobs")]
    [AuthorizePermission(PermissionKeys.Exceptions.Create)]
    public async Task<IActionResult> GetJobs(
        [FromQuery] GetExceptionJobsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("managements")]
    [AuthorizePermission(PermissionKeys.Exceptions.Create)]
    public async Task<IActionResult> GetManagements(
        [FromQuery] GetManagementsPagedQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("departments")]
    [AuthorizePermission(PermissionKeys.Exceptions.Create)]
    public async Task<IActionResult> GetDepartments(
        [FromQuery] GetDepartmentsPagedQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("candidates/by-qid")]
    [AuthorizePermission(PermissionKeys.Exceptions.Create)]
    public async Task<IActionResult> GetCandidateByQid([FromQuery] string qid)
    {
        var result = await mediator.Send(new GetExceptionCandidateByQidQuery(qid));
        return result.ToActionResult();
    }
}
