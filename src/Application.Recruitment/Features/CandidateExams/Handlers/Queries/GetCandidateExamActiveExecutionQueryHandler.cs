using Application.Recruitment.Features.CandidateExams.Common;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Queries;

public sealed class GetCandidateExamActiveExecutionQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCandidateExamActiveExecutionQuery, IResult<CandidateExamActiveExecutionResponse>>
{
    public async Task<IResult<CandidateExamActiveExecutionResponse>> Handle(
        GetCandidateExamActiveExecutionQuery query,
        CancellationToken cancellationToken)
    {
        var attempt = await unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .OwnedByApplicant(query.UserId)
            .Where(item => item.Id == query.TestAttemptId)
            .Select(item => new
            {
                item.Id,
                item.StatusId,
                item.ActiveSessionId,
                CandidateStatusId = item.TestSessionCandidate!.StatusId,
                SessionStatusId = item.TestSessionCandidate.TestSession!.StatusId,
                SlotStatusId = item.TestSessionCandidate.TestSession.TestSlot!.StatusId,
                item.TestSessionCandidate.TestSession.Exam!.AllowPreviousQuestion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (attempt is null)
            return Failure(ErrorsCodes.CandidateExamAttemptNotFound, StatusCodes.Status404NotFound);
        if (attempt.StatusId != TestAttemptStatusIds.InProgress)
            return Failure(CandidateExamAttemptStatusErrors.Resolve(attempt.StatusId));
        if (!attempt.ActiveSessionId.HasValue || attempt.ActiveSessionId.Value == Guid.Empty)
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);
        if (attempt.CandidateStatusId != TestSessionCandidateStatusIds.Started)
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);
        if (attempt.SlotStatusId != TestSlotStatusIds.Started)
            return Failure(ErrorsCodes.CandidateExamSlotNotRunning);
        if (attempt.SessionStatusId != TestSessionStatusIds.InProgress)
            return Failure(ErrorsCodes.CandidateExamSessionNotRunning);

        var runningParts = await unitOfWork.Context.Set<TestAttemptPart>()
            .AsNoTracking()
            .Where(item => item.TestAttemptId == query.TestAttemptId
                           && item.StatusId == TestAttemptPartStatusIds.InProgress)
            .Select(item => new
            {
                item.Id,
                item.StartedAt,
                item.DurationMinutesSnapshot,
                item.EffectiveDeadlineAt,
                item.LastQuestionNo
            })
            .Take(2)
            .ToListAsync(cancellationToken);

        if (runningParts.Count != 1)
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);

        var runningPart = runningParts[0];
        if (runningPart.StartedAt is null || runningPart.DurationMinutesSnapshot is not > 0
            || runningPart.EffectiveDeadlineAt is null || runningPart.LastQuestionNo is not > 0)
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);

        return Result.Ok(new CandidateExamActiveExecutionResponse(
            attempt.Id,
            runningPart.Id,
            attempt.ActiveSessionId.Value,
            runningPart.EffectiveDeadlineAt.Value.AsUtcOffset(),
            runningPart.LastQuestionNo.Value,
            attempt.AllowPreviousQuestion));
    }

    private static IResult<CandidateExamActiveExecutionResponse> Failure(string code, int statusCode = 409) =>
        Result.Fail<CandidateExamActiveExecutionResponse>(new Error(code).WithMetadata("Code", code)
            .WithMetadata("StatusCode", statusCode));
}
