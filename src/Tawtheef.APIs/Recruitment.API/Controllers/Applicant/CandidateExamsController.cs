using System.Security.Claims;
using Application.Recruitment.Features.CandidateExams.Commands;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers.Applicant;

[ApiController]
[Route("api/candidate-exams")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CandidateExamsController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
        {
            null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
            var id when Guid.TryParse(id, out var guid) => Result.Ok(guid),
            _ => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier)
        };

    [HttpGet]
    public async Task<IActionResult> GetCandidateExamAssignments()
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new GetCandidateExamAssignmentsQuery(UserId.Value));

        return result.ToActionResult();
    }

    [HttpPost("{testSessionCandidateId:guid}/validate-access-code")]
    [EnableRateLimiting(LimitsPolicyKeys.VerificationConfirmationPolicy)]
    public async Task<IActionResult> ValidateCandidateExamAccessCode(
        Guid testSessionCandidateId,
        [FromBody] ValidateCandidateExamAccessCodeRequest request,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new ValidateCandidateExamAccessCodeCommand(
                UserId.Value,
                testSessionCandidateId,
                request.AccessCode),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{testSessionCandidateId:guid}/start")]
    [EnableRateLimiting(LimitsPolicyKeys.VerificationConfirmationPolicy)]
    public async Task<IActionResult> StartCandidateExam(
        Guid testSessionCandidateId,
        [FromBody] ValidateCandidateExamAccessCodeRequest request,
        [FromHeader(Name = "X-Exam-Session-Id")] Guid activeSessionId,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        if (activeSessionId == Guid.Empty)
            return BadRequest("X-Exam-Session-Id is required.");

        var result = await mediator.Send(
            new StartCandidateExamCommand(
                UserId.Value,
                testSessionCandidateId,
                request.AccessCode,
                activeSessionId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("attempts/{testAttemptId:guid}/current-question")]
    public async Task<IActionResult> GetCandidateExamCurrentQuestion(
        Guid testAttemptId,
        [FromHeader(Name = "X-Exam-Session-Id")] Guid activeSessionId,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new GetCandidateExamCurrentQuestionQuery(
                UserId.Value,
                activeSessionId,
                testAttemptId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("attempts/{testAttemptId:guid}/active-execution")]
    public async Task<IActionResult> GetCandidateExamActiveExecution(
        Guid testAttemptId,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new GetCandidateExamActiveExecutionQuery(UserId.Value, testAttemptId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("attempts/{testAttemptId:guid}/questions/{testAttemptQuestionId:guid}/answer")]
    public async Task<IActionResult> SaveCandidateExamAnswer(
        Guid testAttemptId,
        Guid testAttemptQuestionId,
        [FromBody] SaveCandidateExamAnswerRequest request,
        [FromHeader(Name = "X-Exam-Session-Id")] Guid activeSessionId,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new SaveCandidateExamAnswerCommand(
                UserId.Value,
                testAttemptId,
                testAttemptQuestionId,
                activeSessionId,
                request.SelectedOptionId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("attempts/{testAttemptId:guid}/navigate")]
    public async Task<IActionResult> NavigateCandidateExam(
        Guid testAttemptId,
        [FromBody] NavigateCandidateExamRequest request,
        [FromHeader(Name = "X-Exam-Session-Id")] Guid activeSessionId,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new NavigateCandidateExamCommand(
                UserId.Value,
                testAttemptId,
                activeSessionId,
                request.TargetQuestionNumber),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("attempts/{testAttemptId:guid}/parts/{testAttemptPartId:guid}/submit")]
    public async Task<IActionResult> SubmitCandidateExamPart(
        Guid testAttemptId,
        Guid testAttemptPartId,
        [FromHeader(Name = "X-Exam-Session-Id")] Guid activeSessionId,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new SubmitCandidateExamPartCommand(
                UserId.Value,
                testAttemptId,
                activeSessionId,
                testAttemptPartId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("attempts/{testAttemptId:guid}/parts/{testAttemptPartId:guid}/timeout")]
    public async Task<IActionResult> TimeoutCandidateExamPart(
        Guid testAttemptId,
        Guid testAttemptPartId,
        [FromHeader(Name = "X-Exam-Session-Id")] Guid activeSessionId,
        CancellationToken cancellationToken)
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(
            new TimeoutCandidateExamPartCommand(
                UserId.Value,
                testAttemptId,
                activeSessionId,
                testAttemptPartId),
            cancellationToken);

        return result.ToActionResult();
    }
}
