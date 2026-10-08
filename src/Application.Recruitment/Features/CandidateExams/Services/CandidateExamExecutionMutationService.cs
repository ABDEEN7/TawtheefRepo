using Application.Recruitment.Features.CandidateExams.Common;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamExecutionMutationService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    CandidateExamScoringService scoringService,
    IApplicationLockService applicationLockService,
    CandidateExamExecutionStateService executionStateService,
    CandidateExamPartTransitionContextService transitionContextService)
{
    public async Task<IResult<SaveCandidateExamAnswerResponse>> SaveAnswerAsync(Guid userId, Guid attemptId,
        Guid activeSessionId, Guid attemptQuestionId, Guid selectedOptionId, CancellationToken cancellationToken)
    {
        if (!await AttemptMatchesApplicantSessionAsync(userId, attemptId, activeSessionId, cancellationToken))
            return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamAttemptNotFound,
                StatusCodes.Status404NotFound);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<SaveCandidateExamAnswerResponse>>(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);
            var stateResult = await executionStateService.LoadCurrentAsync(userId, attemptId, activeSessionId, token);

            if (stateResult.IsFailed)
            {
                return Result.Fail<SaveCandidateExamAnswerResponse>(stateResult.Errors);
            }

            var state = stateResult.Value;
            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (now >= state.Part.EffectiveDeadlineAt!.Value)
                return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamPartTimeExpired);
            if (state.Question.Id != attemptQuestionId)
                return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamCurrentQuestionInvalid);

            var optionIsValid = await unitOfWork.Context.Set<QuestionRevisionOption>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == selectedOptionId && x.QuestionRevisionId == state.Question.QuestionRevisionId,
                    token);
            if (!optionIsValid)
                return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamInvalidQuestionOption);

            if (state.Question.SelectedQuestionRevisionOptionId == selectedOptionId)
            {
                if (state.Question.SavedAt is null)
                    return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamCurrentStateInvalid);
                return Result.Ok(new SaveCandidateExamAnswerResponse(state.Question.Id, selectedOptionId,
                    state.Question.SavedAt.Value.AsUtcOffset(), false));
            }

            state.Question.SelectedQuestionRevisionOptionId = selectedOptionId;
            state.Question.SavedAt = now;
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(new SaveCandidateExamAnswerResponse(state.Question.Id, selectedOptionId,
                now.AsUtcOffset(), true));
        }, cancellationToken);
    }

    public async Task<IResult<NavigateCandidateExamResponse>> NavigateAsync(Guid userId, Guid attemptId,
        Guid activeSessionId, int targetQuestionNumber, CancellationToken cancellationToken)
    {
        if (!await AttemptMatchesApplicantSessionAsync(userId, attemptId, activeSessionId, cancellationToken))
            return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamAttemptNotFound,
                StatusCodes.Status404NotFound);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<NavigateCandidateExamResponse>>(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);
            var stateResult = await executionStateService.LoadCurrentAsync(userId, attemptId, activeSessionId, token);

            if (stateResult.IsFailed)
            {
                return Result.Fail<NavigateCandidateExamResponse>(stateResult.Errors);
            }

            var state = stateResult.Value;
            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (now >= state.Part.EffectiveDeadlineAt!.Value)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamPartTimeExpired);

            var current = state.Part.LastQuestionNo!.Value;
            if (targetQuestionNumber == current)
                return NavigationResult(state, false);
            if (targetQuestionNumber == current + 1 && current == state.TotalQuestions)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamNoNextQuestion);
            if (targetQuestionNumber < 1 || targetQuestionNumber > state.TotalQuestions
                                         || Math.Abs(targetQuestionNumber - current) != 1)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamInvalidNavigationTarget);

            if (targetQuestionNumber == current - 1 && !state.AllowPreviousQuestion)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamPreviousNotAllowed);

            state.Part.LastQuestionNo = targetQuestionNumber;
            await unitOfWork.SaveChangesAsync(token);
            return NavigationResult(state, true);
        }, cancellationToken);
    }

    public async Task<IResult<SubmitCandidateExamPartResponse>> SubmitPartAsync(Guid userId, Guid attemptId,
        Guid activeSessionId, Guid requestedPartId, CancellationToken cancellationToken)
    {
        if (!await AttemptBelongsToApplicantAsync(userId, attemptId, cancellationToken))
        {
            return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamAttemptNotFound,
                StatusCodes.Status404NotFound);
        }

        return await unitOfWork.ExecuteInTransactionAsync<IResult<SubmitCandidateExamPartResponse>>(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);

            var contextResult = await transitionContextService.LoadAsync(userId, attemptId, requestedPartId, token);

            if (contextResult.IsFailed)
            {
                return Result.Fail<SubmitCandidateExamPartResponse>(contextResult.Errors);
            }

            var context = contextResult.Value;

            var attempt = context.Attempt;
            var candidate = context.Candidate;
            var requested = context.Requested;
            var next = context.Next;
            var requestedIndex = context.RequestedIndex;

            var activeSessionMatches = context.ActiveSessionMatches(activeSessionId);

            if (!activeSessionMatches && !context.IsTerminalReplay)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamActiveSessionInvalid);
            }

            if (!activeSessionMatches)
            {
                if (requested.Part.StatusId != TestAttemptPartStatusIds.Completed)
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamActiveSessionInvalid);
                return await ExistingCompleteResult(attempt, candidate, requested, next, context.AllowPreviousQuestion,
                    token);
            }

            // Idempotency must be checked before current attempt/session/deadline checks.
            if (requested.Part.StatusId == TestAttemptPartStatusIds.Completed)
            {
                return await ExistingCompleteResult(attempt, candidate, requested, next, context.AllowPreviousQuestion,
                    token);
            }

            if (attempt.StatusId != TestAttemptStatusIds.InProgress)
            {
                return Failure<SubmitCandidateExamPartResponse>(CandidateExamAttemptStatusErrors.Resolve(
                    attempt.StatusId));
            }

            if (candidate.StatusId != TestSessionCandidateStatusIds.Started)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamCurrentStateInvalid);
            }

            if (context.SlotStatusId != TestSlotStatusIds.Started)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamSlotNotRunning);
            }

            if (context.SessionStatusId != TestSessionStatusIds.InProgress)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamSessionNotRunning);
            }

            if (!context.IsRequestedPartTheOnlyRunningPart())
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartNotCurrent);
            }

            if (!requested.Part.HasValidRunningState)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTransitionStateInvalid);
            }

            var transitionTimeUtc = timeProvider.GetUtcNow().UtcDateTime;

            if (transitionTimeUtc >= requested.Part.EffectiveDeadlineAt!.Value)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTimeExpired);
            }

            var currentQuestionCount = await GetValidPartQuestionCountAsync(requested.Part.Id, token);

            if (!currentQuestionCount.HasValue)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartNotReadyToComplete);
            }

            if (requestedIndex == 0)
            {
                var score = await scoringService.CalculatePartScoreAsync(attemptId, requested.Part.Id,
                    requested.Part.ExamPartId, token);

                if (!score.HasValue || !requested.QualificationScore.HasValue)
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamConfigurationInvalid);
                }

                var passed = score.Value >= requested.QualificationScore.Value;

                requested.Part.Complete(transitionTimeUtc);

                if (!passed)
                {
                    await CompleteAttemptAsync(attempt, candidate, transitionTimeUtc, token);

                    await unitOfWork.SaveChangesAsync(token);

                    return CompleteResult(attemptId, requested, CandidateExamSubmissionOutcomes.Failed, null, null,
                        attempt.StatusId, true, context.AllowPreviousQuestion, true);
                }

                if (next is null || !next.Part.IsCompletelyNotStarted)
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartStateInvalid);
                }

                if (next.DurationMinutes <= 0)
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartDurationInvalid);
                }

                var nextQuestionCount = await GetValidPartQuestionCountAsync(next.Part.Id, token);

                if (!nextQuestionCount.HasValue)
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartQuestionsInvalid);
                }

                // Part 2 starts immediately at the same transition time.
                next.Part.Start(transitionTimeUtc, next.DurationMinutes);

                await unitOfWork.SaveChangesAsync(token);

                return CompleteResult(attemptId, requested, CandidateExamSubmissionOutcomes.Passed, next,
                    nextQuestionCount.Value, attempt.StatusId, false, context.AllowPreviousQuestion, true);
            }

            // PART 2 must be the final part.
            if (requestedIndex != 1 || next is not null)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamConfigurationInvalid);
            }

            // PART 2: no candidate-facing qualification result.
            requested.Part.Complete(transitionTimeUtc);

            await CompleteAttemptAsync(attempt, candidate, transitionTimeUtc, token);

            await unitOfWork.SaveChangesAsync(token);

            return CompleteResult(attemptId, requested, CandidateExamSubmissionOutcomes.Completed, null, null,
                attempt.StatusId, true, context.AllowPreviousQuestion, true);
        }, cancellationToken);
    }

    public async Task<IResult<TimeoutCandidateExamPartResponse>> TimeoutPartAsync(Guid userId, Guid attemptId,
        Guid activeSessionId, Guid requestedPartId, CancellationToken cancellationToken)
    {
        if (!await AttemptBelongsToApplicantAsync(userId, attemptId, cancellationToken))
        {
            return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamAttemptNotFound,
                StatusCodes.Status404NotFound);
        }

        return await unitOfWork.ExecuteInTransactionAsync<IResult<TimeoutCandidateExamPartResponse>>(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);

            var contextResult = await transitionContextService.LoadAsync(userId, attemptId, requestedPartId, token);

            if (contextResult.IsFailed)
            {
                return Result.Fail<TimeoutCandidateExamPartResponse>(contextResult.Errors);
            }

            var context = contextResult.Value;

            var attempt = context.Attempt;
            var candidate = context.Candidate;
            var requested = context.Requested;
            var next = context.Next;
            var requestedIndex = context.RequestedIndex;

            var activeSessionMatches = context.ActiveSessionMatches(activeSessionId);

            if (!activeSessionMatches && !context.IsTerminalReplay)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamActiveSessionInvalid);
            }

            if (!activeSessionMatches)
            {
                if (requested.Part.StatusId != TestAttemptPartStatusIds.TimedOut)
                    return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamActiveSessionInvalid);
                return await ExistingTimeoutResult(attempt, candidate, requested, next, token);
            }

            // Idempotency first.
            if (requested.Part.StatusId == TestAttemptPartStatusIds.TimedOut)
            {
                return await ExistingTimeoutResult(attempt, candidate, requested, next, token);
            }

            if (requested.Part.StatusId == TestAttemptPartStatusIds.Completed)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartAlreadyCompleted);
            }

            if (attempt.StatusId == TestAttemptStatusIds.Interrupted
                || requested.Part.StatusId == TestAttemptPartStatusIds.Interrupted)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamAttemptInterrupted);
            }

            if (attempt.StatusId != TestAttemptStatusIds.InProgress)
            {
                return Failure<TimeoutCandidateExamPartResponse>(CandidateExamAttemptStatusErrors.Resolve(
                    attempt.StatusId));
            }

            if (candidate.StatusId != TestSessionCandidateStatusIds.Started)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamCurrentStateInvalid);
            }

            if (context.SlotStatusId != TestSlotStatusIds.Started)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamSlotNotRunning);
            }

            if (context.SessionStatusId != TestSessionStatusIds.InProgress)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamSessionNotRunning);
            }

            if (!context.IsRequestedPartTheOnlyRunningPart())
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartNotCurrent);
            }

            if (!requested.Part.HasValidRunningState)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            var currentQuestionCount = await GetValidPartQuestionCountAsync(requested.Part.Id, token);

            if (!currentQuestionCount.HasValue || requested.Part.LastQuestionNo > currentQuestionCount.Value)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            var transitionTimeUtc = timeProvider.GetUtcNow().UtcDateTime;

            if (transitionTimeUtc < requested.Part.EffectiveDeadlineAt!.Value)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTimeNotExpired);
            }

            // PART 1
            if (requestedIndex == 0)
            {
                var score = await scoringService.CalculatePartScoreAsync(attemptId, requested.Part.Id,
                    requested.Part.ExamPartId, token);

                if (!score.HasValue || !requested.QualificationScore.HasValue)
                {
                    return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamConfigurationInvalid);
                }


                var passed = score.Value >= requested.QualificationScore.Value;

                if (!passed)
                {
                    var timedOutAtNotPassed = requested.Part.EffectiveDeadlineAt!.Value;

                    requested.Part.MarkTimedOut();

                    await TimeOutAttemptAsync(attempt, candidate, timedOutAtNotPassed, token);

                    await unitOfWork.SaveChangesAsync(token);

                    return TimeoutResult(attemptId, requested, CandidateExamSubmissionOutcomes.Failed, null, null,
                        attempt.StatusId, true, true);
                }

                if (next is null || !next.Part.IsCompletelyNotStarted)
                {
                    return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartStateInvalid);
                }

                if (next.DurationMinutes <= 0)
                {
                    return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartDurationInvalid);
                }

                var nextQuestionCount = await GetValidPartQuestionCountAsync(next.Part.Id, token);

                if (!nextQuestionCount.HasValue)
                {
                    return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartQuestionsInvalid);
                }

                var nextPartStartedAt = requested.Part.EffectiveDeadlineAt!.Value;

                requested.Part.MarkTimedOut();

                next.Part.Start(nextPartStartedAt, next.DurationMinutes);

                await unitOfWork.SaveChangesAsync(token);

                return TimeoutResult(attemptId, requested, CandidateExamSubmissionOutcomes.Passed, next,
                    nextQuestionCount.Value, attempt.StatusId, false, true);
            }

            // Part 2 must be the final part.
            if (next is not null)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamConfigurationInvalid);
            }

            var timedOutAt = requested.Part.EffectiveDeadlineAt!.Value;

            requested.Part.MarkTimedOut();

            await TimeOutAttemptAsync(attempt, candidate, timedOutAt, token);

            await unitOfWork.SaveChangesAsync(token);

            return TimeoutResult(attemptId, requested, CandidateExamSubmissionOutcomes.Completed, null, null,
                attempt.StatusId, true, true);
        }, cancellationToken);
    }

    private async Task CompleteAttemptAsync(TestAttempt attempt, TestSessionCandidate candidate, DateTime completedAt,
        CancellationToken cancellationToken)
    {
        attempt.Complete(completedAt);

        candidate.MarkExamCompleted();

        await DeleteAttemptConnectionsAsync(attempt.Id, cancellationToken);
    }

    private async Task TimeOutAttemptAsync(TestAttempt attempt, TestSessionCandidate candidate, DateTime completedAt,
        CancellationToken cancellationToken)
    {
        attempt.MarkTimedOut(completedAt);

        candidate.MarkExamCompleted();

        await DeleteAttemptConnectionsAsync(attempt.Id, cancellationToken);
    }

    private Task<int> DeleteAttemptConnectionsAsync(Guid attemptId, CancellationToken cancellationToken) =>
        unitOfWork.Context.Set<CandidateExamConnection>()
            .Where(x => x.TestAttemptId == attemptId)
            .ExecuteDeleteAsync(cancellationToken);

    private async Task<IResult<TimeoutCandidateExamPartResponse>> ExistingTimeoutResult(TestAttempt attempt,
        TestSessionCandidate candidate, CandidateExamPartTransitionData requested,
        CandidateExamPartTransitionData? next, CancellationToken token)
    {
        if (requested.Part.EffectiveDeadlineAt is null
            || requested.Part.CompletedAt != requested.Part.EffectiveDeadlineAt)
        {
            return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamTimeoutStateInvalid);
        }

        // Final part already timed out.
        if (next is null)
        {
            if (attempt.StatusId != TestAttemptStatusIds.TimedOut
                || attempt.CompletedAt != requested.Part.EffectiveDeadlineAt
                || candidate.StatusId != TestSessionCandidateStatusIds.Completed)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            return TimeoutResult(attempt.Id, requested, CandidateExamSubmissionOutcomes.Completed, null, null,
                attempt.StatusId, true, false);
        }

        // Part 1 passed previously, so Part 2 was started.
        if (next.Part.HasValidExecutionHistory)
        {
            var count = await GetValidPartQuestionCountAsync(next.Part.Id, token);

            if (!count.HasValue)
            {
                return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            return TimeoutResult(attempt.Id, requested, CandidateExamSubmissionOutcomes.Passed, next, count.Value,
                attempt.StatusId, attempt.StatusId != TestAttemptStatusIds.InProgress, false);
        }

        // Part 1 failed previously, so Part 2 never started.
        if (attempt.StatusId == TestAttemptStatusIds.TimedOut
            && candidate.StatusId == TestSessionCandidateStatusIds.Completed && next.Part.IsCompletelyNotStarted)
        {
            return TimeoutResult(attempt.Id, requested, CandidateExamSubmissionOutcomes.Failed, null, null,
                attempt.StatusId, true, false);
        }

        return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamTimeoutStateInvalid);
    }

    private static StartedCandidateExamPartDto? CreateStartedPartDto(CandidateExamPartTransitionData? part,
        int? totalQuestions)
    {
        if (part is null)
        {
            return null;
        }

        if (totalQuestions is not > 0 || part.Part.StartedAt is null || part.Part.DurationMinutesSnapshot is not > 0
            || part.Part.EffectiveDeadlineAt is null || part.Part.LastQuestionNo is not > 0)
        {
            throw new InvalidOperationException("A started attempt part must have a complete execution state.");
        }

        return new StartedCandidateExamPartDto(part.Part.Id, part.PartNo, part.Part.StartedAt.Value.AsUtcOffset(),
            part.Part.DurationMinutesSnapshot.Value, part.Part.EffectiveDeadlineAt.Value.AsUtcOffset(),
            part.Part.LastQuestionNo.Value, totalQuestions.Value);
    }

    private static IResult<TimeoutCandidateExamPartResponse> TimeoutResult(Guid attemptId,
        CandidateExamPartTransitionData timedOut, string outcome, CandidateExamPartTransitionData? next,
        int? nextQuestionCount, Guid attemptStatusId, bool examEnded, bool changed)
    {
        var nextPart = CreateStartedPartDto(next, nextQuestionCount);

        return Result.Ok(new TimeoutCandidateExamPartResponse(attemptId,
            new TimedOutCandidateExamPartDto(timedOut.Part.Id, timedOut.PartNo,
                timedOut.Part.CompletedAt!.Value.AsUtcOffset()),
            outcome, next is not null, nextPart, attemptStatusId, examEnded, changed));
    }

    private async Task<int?> GetValidPartQuestionCountAsync(Guid partId, CancellationToken cancellationToken)
    {
        var statistics = await unitOfWork.Context.Set<TestAttemptQuestion>()
            .Where(x => x.TestAttemptPartId == partId)
            .GroupBy(x => x.TestAttemptPartId)
            .Select(group => new
            {
                Count = group.Count(),
                MinOrderNo = group.Min(x => x.OrderNo),
                MaxOrderNo = group.Max(x => x.OrderNo)
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (statistics is null || statistics.Count == 0 || statistics.MinOrderNo != 1
            || statistics.MaxOrderNo != statistics.Count)
        {
            return null;
        }

        return statistics.Count;
    }

    private static IResult<SubmitCandidateExamPartResponse> CompleteResult(Guid attemptId,
        CandidateExamPartTransitionData completed, string outcome, CandidateExamPartTransitionData? next,
        int? nextQuestionCount, Guid attemptStatusId, bool examEnded, bool allowPreviousQuestion, bool changed)
    {
        var nextPart = CreateStartedPartDto(next, nextQuestionCount);

        return Result.Ok(new SubmitCandidateExamPartResponse(attemptId,
            new CompletedCandidateExamPartDto(completed.Part.Id, completed.PartNo,
                completed.Part.CompletedAt!.Value.AsUtcOffset()),
            outcome, next is not null, nextPart, attemptStatusId, examEnded, allowPreviousQuestion, changed));
    }

    private Task<bool> AttemptBelongsToApplicantAsync(
        Guid userId, Guid attemptId, CancellationToken cancellationToken) =>
        unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .OwnedByApplicant(userId)
            .AnyAsync(x => x.Id == attemptId, cancellationToken);

    private Task<bool> AttemptMatchesApplicantSessionAsync(Guid userId, Guid attemptId, Guid activeSessionId,
        CancellationToken cancellationToken) =>
        unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .OwnedByApplicant(userId)
            .AnyAsync(x => x.Id == attemptId && x.ActiveSessionId == activeSessionId, cancellationToken);

    private async Task<IResult<SubmitCandidateExamPartResponse>> ExistingCompleteResult(TestAttempt attempt,
        TestSessionCandidate candidate, CandidateExamPartTransitionData requested,
        CandidateExamPartTransitionData? next, bool allowPreviousQuestion, CancellationToken token)
    {
        if (requested.Part.CompletedAt is null)
        {
            return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTransitionStateInvalid);
        }

        // Final part was already submitted successfully.
        if (next is null)
        {
            if (attempt.StatusId != TestAttemptStatusIds.Completed
                || candidate.StatusId != TestSessionCandidateStatusIds.Completed)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTransitionStateInvalid);
            }

            return CompleteResult(attempt.Id, requested, CandidateExamSubmissionOutcomes.Completed, null, null,
                attempt.StatusId, true, allowPreviousQuestion, false);
        }

        // Part 1 passed previously: Part 2 has started.
        if (next.Part.HasValidExecutionHistory)
        {
            var nextQuestionCount = await GetValidPartQuestionCountAsync(next.Part.Id, token);

            if (!nextQuestionCount.HasValue)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTransitionStateInvalid);
            }

            return CompleteResult(attempt.Id, requested, CandidateExamSubmissionOutcomes.Passed, next,
                nextQuestionCount.Value, attempt.StatusId, attempt.StatusId != TestAttemptStatusIds.InProgress,
                allowPreviousQuestion, false);
        }

        // Part 1 failed previously: Part 2 never started.
        if (attempt.StatusId == TestAttemptStatusIds.Completed
            && candidate.StatusId == TestSessionCandidateStatusIds.Completed && next.Part.IsCompletelyNotStarted)
        {
            return CompleteResult(attempt.Id, requested, CandidateExamSubmissionOutcomes.Failed, null, null,
                attempt.StatusId, true, allowPreviousQuestion, false);
        }

        return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTransitionStateInvalid);
    }

    private static IResult<NavigateCandidateExamResponse> NavigationResult(CandidateExamExecutionState state,
        bool changed)
    {
        var current = state.Part.LastQuestionNo!.Value;
        return Result.Ok(new NavigateCandidateExamResponse(state.AttemptId, state.Part.Id, current,
            state.TotalQuestions, state.AllowPreviousQuestion && current > 1, current < state.TotalQuestions,
            state.AllowPreviousQuestion, changed));
    }

    private static IResult<T> Failure<T>(string code, int statusCode = 409) =>
        Result.Fail<T>(new Error(code).WithMetadata("Code", code).WithMetadata("StatusCode", statusCode));
}
