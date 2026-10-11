using Application.Operation.Features.Employee.Exams.Commands;
using Application.Operation.Features.Employee.Exams.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;
using Application.Operation.Features.Employee.Exams.Queries;
using Application.Operation.Features.Employee.Exams.ResultReport.Queries;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/exam-result-reports")]
[Microsoft.AspNetCore.Authorization.Authorize(
    AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
public sealed class ExamResultReportsController(
    IMediator mediator)
    : ControllerBase
{
    [HttpPost("{reportId:guid}/candidates/{candidateResultId:guid}/accept-calculated-score")]
    [AuthorizePermission(PermissionKeys.Exams.WorkflowActions)]
    public async Task<IActionResult> AcceptCalculatedScore(
        Guid reportId,
        Guid candidateResultId,
        [FromBody] AcceptExamResultCandidateCalculatedScoreRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new AcceptExamResultCandidateCalculatedScoreCommand(
                reportId, candidateResultId, request.ResolutionNotes),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{reportId:guid}/candidates/{candidateResultId:guid}/reschedule")]
    [AuthorizePermission(PermissionKeys.Exams.WorkflowActions)]
    public async Task<IActionResult> RescheduleCandidate(
        Guid reportId,
        Guid candidateResultId,
        [FromBody] RescheduleExamResultCandidateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RescheduleExamResultCandidateCommand(
                reportId, candidateResultId, request.TargetTestSessionId, request.Reason),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{reportId:guid}/candidates/{candidateResultId:guid}/reschedule-options")]
    [AuthorizePermission(PermissionKeys.Exams.WorkflowActions)]
    public async Task<IActionResult> RescheduleOptions(
        Guid reportId, Guid candidateResultId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetExamResultCandidateRescheduleOptionsQuery(reportId, candidateResultId),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{reportId:guid}/candidates/{candidateResultId:guid}/resolve-not-completed")]
    [AuthorizePermission(PermissionKeys.Exams.WorkflowActions)]
    public async Task<IActionResult> ResolveNotCompleted(
        Guid reportId,
        Guid candidateResultId,
        [FromBody] ResolveExamResultCandidateAsNotCompletedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ResolveExamResultCandidateAsNotCompletedCommand(
                reportId, candidateResultId, request.ResolutionNotes),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{reportId:guid}/return-for-processing")]
    [AuthorizePermission(PermissionKeys.Exams.WorkflowActions)]
    public async Task<IActionResult> ReturnForProcessing(
        Guid reportId,
        [FromBody] ReturnExamResultReportForProcessingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ReturnExamResultReportForProcessingCommand(reportId, request.Reason),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{reportId:guid}/approve")]
    [AuthorizePermission(PermissionKeys.Exams.ApproveResults)]
    public async Task<IActionResult> Approve(Guid reportId,[FromBody] ApproveExamResultReportRequest request,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ApproveExamResultReportCommand(reportId,request.DecisionNotes),cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> List([FromQuery] ListExamResultReportsQuery query,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query,cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{reportId:guid}")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> Details(Guid reportId,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetExamResultReportDetailsQuery(reportId),cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{reportId:guid}/candidates/{candidateResultId:guid}/attempt")]
    [AuthorizePermission(PermissionKeys.Exams.ViewAttemptDetails)]
    public async Task<IActionResult> CandidateAttempt( Guid reportId,Guid candidateResultId,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetExamResultCandidateAttemptDetailsQuery(reportId, candidateResultId),
            cancellationToken);
        return result.ToActionResult();
    }
}
