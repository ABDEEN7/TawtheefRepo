using Application.Recruitment.Features.CandidateExams.Commands;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Commands;

public sealed class StartCandidateExamCommandHandler(
    CandidateExamEntryValidationService entryValidation,
    CandidateExamAttemptPreparationService attemptPreparation,
    CandidateExamQuestionGenerationService questionGeneration,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<StartCandidateExamCommand, IResult<StartCandidateExamResponse>>
{
    public async Task<IResult<StartCandidateExamResponse>> Handle(
        StartCandidateExamCommand command,
        CancellationToken cancellationToken)
    {
        var initial = await entryValidation.ValidateForResumeAsync(
            command.UserId,
            command.TestSessionCandidateId,
            command.AccessCode,
            cancellationToken);

        if (initial.IsFailed)
            return Result.Fail<StartCandidateExamResponse>(initial.Errors);

        var testSlotId = await unitOfWork.Context.Set<TestSession>()
            .AsNoTracking()
            .Where(session => session.Id == initial.Value.TestSessionId)
            .Select(session => session.TestSlotId)
            .SingleOrDefaultAsync(cancellationToken);
        if (testSlotId is null)
            return Failure(ErrorsCodes.CandidateExamAssignmentNotFound);

        if (initial.Value.CandidateStatusId == TestSessionCandidateStatusIds.Authorized)
        {
            var preparation = await attemptPreparation.PrepareAsync(
                command.TestSessionCandidateId,
                testSlotId.Value,
                cancellationToken);

            if (preparation.IsFailed)
            {
                if (!HasError(preparation, ErrorsCodes.CandidateExamAttemptAlreadyStarted))
                    return Result.Fail<StartCandidateExamResponse>(preparation.Errors);
            }
            else
            {
                var generation = await questionGeneration.GenerateAsync(
                    preparation.Value.TestAttemptId,
                    cancellationToken);

                if (generation.IsFailed &&
                    !HasError(generation, ErrorsCodes.CandidateExamAttemptAlreadyStarted))
                {
                    return Result.Fail<StartCandidateExamResponse>(generation.Errors);
                }
            }
        }

        return await unitOfWork.ExecuteInTransactionAsync<IResult<StartCandidateExamResponse>>(
            async token =>
            {
                await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                    "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                    "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
                    "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam slot start lock unavailable', 1;",
                    [$"Tawtheef.CandidateExam.TestSlot.{testSlotId.Value}"],
                    token);

                await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                    "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                    "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
                    "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam start lock unavailable', 1;",
                    [$"Tawtheef.CandidateExam.TestSessionCandidate.{command.TestSessionCandidateId}"],
                    token);

                var current = await entryValidation.ValidateForResumeAsync(
                    command.UserId,
                    command.TestSessionCandidateId,
                    command.AccessCode,
                    token);

                if (current.IsFailed)
                    return Result.Fail<StartCandidateExamResponse>(current.Errors);

                var candidate = await unitOfWork.Context
                    .Set<TestSessionCandidate>()
                    .Include(x => x.TestSession)
                    .SingleOrDefaultAsync(x => x.Id == command.TestSessionCandidateId, token);

                var execution = await unitOfWork.Context.Set<TestAttempt>()
                    .Where(x => x.TestSessionCandidateId == command.TestSessionCandidateId)
                    .Select(x => new
                    {
                        Attempt = x,
                        x.TestSessionCandidate!.TestSession!.Exam!.AllowPreviousQuestion,
                        Parts = x.Parts
                            .Select(p => new { AttemptPart = p, p.ExamPart!.PartNo, p.ExamPart.DurationMinutes })
                            .OrderBy(p => p.PartNo)
                            .ToList()
                    })
                    .SingleOrDefaultAsync(token);

                if (candidate is null || execution is null || execution.Parts.Count == 0)
                    return Failure(ErrorsCodes.CandidateExamStartStateInvalid);

                var session = candidate.TestSession;

                if (session is null)
                {
                    return Failure(
                        ErrorsCodes.CandidateExamStartStateInvalid);
                }

                if (candidate.StatusId ==
                    TestSessionCandidateStatusIds.Started)
                {
                    if (session.StatusId !=
                        TestSessionStatusIds.InProgress)
                    {
                        return Failure(
                            ErrorsCodes.CandidateExamStartStateInvalid);
                    }
                }
                else if (
                    session.StatusId !=
                    TestSessionStatusIds.Approved &&
                    session.StatusId !=
                    TestSessionStatusIds.Ready &&
                    session.StatusId !=
                    TestSessionStatusIds.InProgress)
                {
                    return Failure(
                        ErrorsCodes.CandidateExamStartStateInvalid);
                }

                var first = execution.Parts[0];

                // Same candidate, same session identifier, same active first part:
                // return the original start response without changing its deadline.
                if (candidate.StatusId == TestSessionCandidateStatusIds.Started)
                {
                    if (execution.Attempt.StatusId != TestAttemptStatusIds.InProgress ||
                        execution.Attempt.ActiveSessionId != command.ActiveSessionId ||
                        execution.Attempt.StartedAt is null ||
                        execution.Attempt.CompletedAt is not null ||
                        first.AttemptPart.StatusId != TestAttemptPartStatusIds.InProgress ||
                        first.AttemptPart.StartedAt is null ||
                        first.AttemptPart.EffectiveDeadlineAt is null ||
                        first.AttemptPart.DurationMinutesSnapshot is null ||
                        first.AttemptPart.LastQuestionNo is null ||
                        first.AttemptPart.EffectiveDeadlineAt <= timeProvider.GetUtcNow().UtcDateTime ||
                        execution.Parts.Skip(1).Any(x => !IsUnstarted(x.AttemptPart)))
                    {
                        return Failure(ErrorsCodes.CandidateExamStartStateInvalid);
                    }

                    return Success(
                        candidate.Id,
                        current.Value.ExamId,
                        execution.Attempt,
                        first.AttemptPart,
                        first.PartNo,
                        execution.AllowPreviousQuestion);
                }

                var completeSet = await questionGeneration.ValidateCompleteSetAsync(
                    execution.Attempt.Id,
                    token);

                if (completeSet.IsFailed)
                    return Result.Fail<StartCandidateExamResponse>(completeSet.Errors);

                if (candidate.StatusId != TestSessionCandidateStatusIds.Authorized ||
                    execution.Attempt.StatusId != TestAttemptStatusIds.NotStarted ||
                    execution.Attempt.StartedAt is not null ||
                    execution.Attempt.CompletedAt is not null ||
                    execution.Parts.Any(x => !IsUnstarted(x.AttemptPart)))
                {
                    return Failure(AttemptError(execution.Attempt.StatusId));
                }

                if (first.DurationMinutes <= 0)
                    return Failure(ErrorsCodes.CandidateExamFirstPartDurationInvalid);

                if (execution.Attempt.ActiveSessionId is not null)
                    return Failure(ErrorsCodes.CandidateExamStartStateInvalid);
                
                if (session.StatusId == TestSessionStatusIds.Approved || session.StatusId == TestSessionStatusIds.Ready)
                {
                    session.StatusId = TestSessionStatusIds.InProgress;
                }

                var startTimeUtc = timeProvider.GetUtcNow().UtcDateTime;

                execution.Attempt.ActiveSessionId = command.ActiveSessionId;
                execution.Attempt.StatusId = TestAttemptStatusIds.InProgress;
                execution.Attempt.StartedAt = startTimeUtc;

                candidate.StatusId = TestSessionCandidateStatusIds.Started;

                first.AttemptPart.StatusId = TestAttemptPartStatusIds.InProgress;
                first.AttemptPart.StartedAt = startTimeUtc;
                first.AttemptPart.DurationMinutesSnapshot = first.DurationMinutes;
                first.AttemptPart.EffectiveDeadlineAt =
                    startTimeUtc.AddMinutes(first.DurationMinutes);
                first.AttemptPart.LastQuestionNo = 1;

                await unitOfWork.SaveChangesAsync(token);

                return Success(
                    candidate.Id,
                    current.Value.ExamId,
                    execution.Attempt,
                    first.AttemptPart,
                    first.PartNo,
                    execution.AllowPreviousQuestion);
            },
            cancellationToken);
    }

    private static bool IsUnstarted(TestAttemptPart part) =>
        part.StatusId == TestAttemptPartStatusIds.NotStarted &&
        part.StartedAt is null &&
        part.CompletedAt is null &&
        part.DurationMinutesSnapshot is null &&
        part.EffectiveDeadlineAt is null &&
        part.LastQuestionNo is null;

    private static IResult<StartCandidateExamResponse> Success(
        Guid candidateId,
        Guid examId,
        TestAttempt attempt,
        TestAttemptPart part,
        int partNo,
        bool allowPreviousQuestion) =>
        Result.Ok(new StartCandidateExamResponse(
            candidateId,
            attempt.Id,
            part.Id,
            examId,
            attempt.ActiveSessionId!.Value,
            partNo,
            part.LastQuestionNo!.Value,
            part.StartedAt!.Value,
            part.EffectiveDeadlineAt!.Value,
            part.DurationMinutesSnapshot!.Value,
            allowPreviousQuestion));

    private static string AttemptError(Guid statusId) =>
        statusId == TestAttemptStatusIds.Interrupted
            ? ErrorsCodes.CandidateExamAttemptInterrupted
            : statusId == TestAttemptStatusIds.Completed
                ? ErrorsCodes.CandidateExamAttemptCompleted
                : statusId == TestAttemptStatusIds.TimedOut
                    ? ErrorsCodes.CandidateExamAttemptTimedOut
                    : statusId == TestAttemptStatusIds.Cancelled
                        ? ErrorsCodes.CandidateExamAttemptCancelled
                        : ErrorsCodes.CandidateExamStartStateInvalid;

    private static IResult<StartCandidateExamResponse> Failure(string code) =>
        Result.Fail<StartCandidateExamResponse>(
            new Error(code)
                .WithMetadata("Code", code)
                .WithMetadata("StatusCode", 409));

    private static bool HasError<T>(IResult<T> result, string code) =>
        result.Errors.Any(error =>
            error.Message == code ||
            error.Metadata.TryGetValue("Code", out var value) &&
            Equals(value, code));
}
