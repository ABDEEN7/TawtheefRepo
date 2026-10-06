using Application.Recruitment.Features.CandidateExams.Common;
using Application.Recruitment.Features.CandidateExams.Models;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamExecutionStateService(IUnitOfWork unitOfWork)
{
    internal async Task<IResult<CandidateExamExecutionState>> LoadCurrentAsync(Guid userId, Guid attemptId,
        Guid activeSessionId, CancellationToken cancellationToken)
    {
        var attempt = await unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .OwnedByApplicant(userId)
            .Where(x => x.Id == attemptId)
            .Select(x => new
            {
                x.Id,
                x.StatusId,
                x.ActiveSessionId,
                CandidateStatusId = x.TestSessionCandidate!.StatusId,
                SessionStatusId = x.TestSessionCandidate.TestSession!.StatusId,
                SlotStatusId = x.TestSessionCandidate.TestSession.TestSlot!.StatusId,
                x.TestSessionCandidate.TestSession.Exam!.AllowPreviousQuestion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (attempt is null)
        {
            return Failure(ErrorsCodes.CandidateExamAttemptNotFound);
        }

        if (attempt.StatusId != TestAttemptStatusIds.InProgress)
        {
            return Failure(CandidateExamAttemptStatusErrors.Resolve(attempt.StatusId));
        }

        if (attempt.CandidateStatusId != TestSessionCandidateStatusIds.Started)
        {
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);
        }

        if (attempt.SlotStatusId != TestSlotStatusIds.Started)
        {
            return Failure(ErrorsCodes.CandidateExamSlotNotRunning);
        }

        if (attempt.SessionStatusId != TestSessionStatusIds.InProgress)
        {
            return Failure(ErrorsCodes.CandidateExamSessionNotRunning);
        }

        if (attempt.ActiveSessionId != activeSessionId)
        {
            return Failure(ErrorsCodes.CandidateExamActiveSessionInvalid);
        }

        var runningParts = await unitOfWork.Context.Set<TestAttemptPart>()
            .Where(x => x.TestAttemptId == attemptId && x.StatusId == TestAttemptPartStatusIds.InProgress)
            .Select(x => new { Part = x, TotalQuestions = x.Questions.Count })
            .Take(2)
            .ToListAsync(cancellationToken);

        if (runningParts.Count != 1)
        {
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);
        }

        var runningPart = runningParts[0];
        var part = runningPart.Part;

        if (!part.HasValidRunningState)
        {
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);
        }

        if (runningPart.TotalQuestions == 0 || part.LastQuestionNo > runningPart.TotalQuestions)
        {
            return Failure(ErrorsCodes.CandidateExamCurrentQuestionInvalid);
        }

        var currentQuestions = await unitOfWork.Context.Set<TestAttemptQuestion>()
            .Where(x => x.TestAttemptId == attemptId && x.TestAttemptPartId == part.Id && part.LastQuestionNo != null
                        && x.OrderNo == part.LastQuestionNo.Value)
            .Take(2)
            .ToListAsync(cancellationToken);

        if (currentQuestions.Count != 1)
        {
            return Failure(ErrorsCodes.CandidateExamCurrentQuestionInvalid);
        }

        return Result.Ok(new CandidateExamExecutionState(attempt.Id, part, currentQuestions[0],
            runningPart.TotalQuestions, attempt.AllowPreviousQuestion));
    }

    private static IResult<CandidateExamExecutionState> Failure(string errorCode) =>
        Result.Fail<CandidateExamExecutionState>(new Error(errorCode).WithMetadata("Code", errorCode)
            .WithMetadata("StatusCode", 409));
}
