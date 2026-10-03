using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamAttemptPreparationService(IUnitOfWork unitOfWork)
{
    public Task<IResult<CandidateExamAttemptPreparationResult>> PrepareAsync(
        Guid testSessionCandidateId,
        Guid testSlotId,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<CandidateExamAttemptPreparationResult>>(async token =>
        {
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
                "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam slot preparation lock unavailable', 1;",
                [$"Tawtheef.CandidateExam.TestSlot.{testSlotId}"], token);

            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
                "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam attempt lock unavailable', 1;",
                [$"Tawtheef.CandidateExam.TestSessionCandidate.{testSessionCandidateId}"], token);

            var examId = await unitOfWork.Context.Set<TestSessionCandidate>()
                .AsNoTracking()
                .Where(candidate => candidate.Id == testSessionCandidateId &&
                                    candidate.StatusId == TestSessionCandidateStatusIds.Authorized &&
                                    candidate.TestSession!.TestSlotId == testSlotId &&
                                    (
                                        candidate.TestSession.StatusId == TestSessionStatusIds.Approved ||
                                        candidate.TestSession.StatusId == TestSessionStatusIds.Ready ||
                                        candidate.TestSession.StatusId == TestSessionStatusIds.InProgress
                                    ) &&
                                    candidate.TestSession.TestSlot!.StatusId == TestSlotStatusIds.Started)
                .Select(candidate => (Guid?)candidate.TestSession!.ExamId)
                .SingleOrDefaultAsync(token);

            if (!examId.HasValue)
                return Result.Fail<CandidateExamAttemptPreparationResult>(
                    ErrorsCodes.CandidateExamAssignmentNotFound);

            var examPartIds = await unitOfWork.Context.Set<ExamPart>()
                .AsNoTracking()
                .Where(part => part.ExamId == examId.Value)
                .OrderBy(part => part.PartNo)
                .Select(part => part.Id)
                .ToListAsync(token);

            if (examPartIds.Count == 0)
                return Result.Fail<CandidateExamAttemptPreparationResult>(
                    ErrorsCodes.CandidateExamHasNoParts);

            var existingAttempt = await unitOfWork.Context.Set<TestAttempt>()
                .AsNoTracking()
                .Where(attempt => attempt.TestSessionCandidateId == testSessionCandidateId)
                .Select(attempt => new
                {
                    attempt.Id,
                    attempt.StatusId,
                    PartExamIds = attempt.Parts.Select(part => part.ExamPartId).ToList()
                })
                .SingleOrDefaultAsync(token);

            if (existingAttempt is not null)
                return ExistingAttemptResult(existingAttempt.Id, existingAttempt.StatusId,
                    existingAttempt.PartExamIds, examPartIds);

            var attempt = new TestAttempt
            {
                TestSessionCandidateId = testSessionCandidateId,
                StatusId = TestAttemptStatusIds.NotStarted,
                StartedAt = null,
                CompletedAt = null
            };

            await unitOfWork.Context.Set<TestAttempt>().AddAsync(attempt, token);

            var existingResult = await unitOfWork.Context.Set<ExamResultCandidate>()
                .SingleOrDefaultAsync(x => x.TestSessionCandidateId == testSessionCandidateId, token);

            if (existingResult is not null)
            {
                if (existingResult.ResultStatusId !=
                    ExamResultCandidateResultStatusIds.RequiresAction)
                {
                    return Result.Fail<
                        CandidateExamAttemptPreparationResult>(
                        ErrorsCodes.CandidateExamCurrentStateInvalid);
                }

                existingResult.TestAttempt = attempt;
            }

            var attemptParts = examPartIds.Select(examPartId => new TestAttemptPart
            {
                TestAttempt = attempt,
                ExamPartId = examPartId,
                StatusId = TestAttemptPartStatusIds.NotStarted,
                StartedAt = null,
                CompletedAt = null,
                LastQuestionNo = null,
                DurationMinutesSnapshot = null,
                EffectiveDeadlineAt = null
            }).ToList();

            await unitOfWork.Context.Set<TestAttemptPart>().AddRangeAsync(attemptParts, token);
            await unitOfWork.SaveChangesAsync(token);

            return Result.Ok(new CandidateExamAttemptPreparationResult(
                attempt.Id,
                true,
                attempt.StatusId,
                attemptParts.Count));
        }, cancellationToken);

    private static IResult<CandidateExamAttemptPreparationResult> ExistingAttemptResult(
        Guid attemptId,
        Guid statusId,
        IReadOnlyCollection<Guid> existingPartExamIds,
        IReadOnlyCollection<Guid> currentExamPartIds)
    {
        if (statusId == TestAttemptStatusIds.NotStarted)
        {
            var structurallyComplete = existingPartExamIds.Count == currentExamPartIds.Count &&
                                       existingPartExamIds.Distinct().Count() == existingPartExamIds.Count &&
                                       !currentExamPartIds.Except(existingPartExamIds).Any() &&
                                       !existingPartExamIds.Except(currentExamPartIds).Any();

            return structurallyComplete
                ? Result.Ok(new CandidateExamAttemptPreparationResult(
                    attemptId,
                    false,
                    statusId,
                    existingPartExamIds.Count))
                : Result.Fail<CandidateExamAttemptPreparationResult>(
                    ErrorsCodes.CandidateExamAttemptStructureInvalid);
        }

        if (statusId == TestAttemptStatusIds.InProgress)
            return Result.Fail<CandidateExamAttemptPreparationResult>(
                ErrorsCodes.CandidateExamAttemptAlreadyStarted);

        if (statusId == TestAttemptStatusIds.Interrupted)
            return Result.Fail<CandidateExamAttemptPreparationResult>(
                ErrorsCodes.CandidateExamAttemptInterrupted);

        if (statusId == TestAttemptStatusIds.Completed)
            return Result.Fail<CandidateExamAttemptPreparationResult>(
                ErrorsCodes.CandidateExamAttemptCompleted);

        if (statusId == TestAttemptStatusIds.TimedOut)
            return Result.Fail<CandidateExamAttemptPreparationResult>(
                ErrorsCodes.CandidateExamAttemptTimedOut);

        return Result.Fail<CandidateExamAttemptPreparationResult>(statusId == TestAttemptStatusIds.Cancelled
            ? ErrorsCodes.CandidateExamAttemptCancelled
            : ErrorsCodes.CandidateExamAttemptStatusInvalid);
    }
}
