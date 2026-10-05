using System.Security.Claims;
using Application.Operation.Features.Employee.CandidateExamAttempts.Commands;
using Application.Operation.Features.Employee.CandidateExamAttempts.DTOs;
using Application.Operation.Features.Employee.CandidateExamAttempts.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/candidate-exam-attempts")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CandidateExamAttemptsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{testAttemptId:guid}/authorize-resume")]
    [AuthorizePermission(PermissionKeys.TestSlots.ManagePeriod)]
    public async Task<IActionResult> AuthorizeResume(Guid testAttemptId,
        [FromBody] AuthorizeCandidateExamResumeRequest request, CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdValue, out var officerId))
            return Unauthorized(Result.Fail(ErrorsCodes.InvalidUserIdentifier).Errors);

        var result =
            await mediator.Send(
                new AuthorizeCandidateExamResumeCommand(testAttemptId, officerId, request.ResolutionNotes),
                cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{testAttemptId:guid}/operational-issue")]
    [AuthorizePermission(PermissionKeys.TestSlots.ManagePeriod)]
    public async Task<IActionResult> ReportOperationalIssue(Guid testAttemptId,
        [FromBody] ReportCandidateExamOperationalIssueRequest request, CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var officerId))
        {
            return Unauthorized(Result.Fail(ErrorsCodes.InvalidUserIdentifier).Errors);
        }

        var result = await mediator.Send(
            new ReportCandidateExamOperationalIssueCommand(testAttemptId, officerId, request.Reason),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{testAttemptId:guid}/reschedule-options")]
    [AuthorizePermission(PermissionKeys.TestSlots.ManagePeriod)]
    public async Task<IActionResult> RescheduleOptions(Guid testAttemptId, CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var officerId))
        {
            return Unauthorized(Result.Fail(ErrorsCodes.InvalidUserIdentifier).Errors);
        }

        var result = await mediator.Send(new GetCandidateExamRescheduleOptionsQuery(testAttemptId, officerId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{testAttemptId:guid}/reschedule")]
    [AuthorizePermission(PermissionKeys.TestSlots.ManagePeriod)]
    public async Task<IActionResult> Reschedule(Guid testAttemptId,
        [FromBody] RescheduleInterruptedCandidateExamRequest request, CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var officerId))
        {
            return Unauthorized(Result.Fail(ErrorsCodes.InvalidUserIdentifier).Errors);
        }

        var result =
            await mediator.Send(
                new RescheduleInterruptedCandidateExamCommand(testAttemptId, officerId, request.TargetTestSessionId,
                    request.Reason), cancellationToken);

        return result.ToActionResult();
    }
}
