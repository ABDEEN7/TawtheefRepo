using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamExecutionMutationService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    CandidateExamScoringService scoringService)
{
    public async Task<IResult<SaveCandidateExamAnswerResponse>> SaveAnswerAsync(Guid userId, Guid attemptId,
        Guid activeSessionId,
        Guid attemptQuestionId, Guid selectedOptionId, CancellationToken cancellationToken)
    {
        if (!await AttemptIsOwnedAsync(userId, attemptId, activeSessionId, cancellationToken))
            return Failure<SaveCandidateExamAnswerResponse>(
                ErrorsCodes.CandidateExamAttemptNotFound, StatusCodes.Status404NotFound);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<SaveCandidateExamAnswerResponse>>(async token =>
        {
            await AcquireAttemptLockAsync(attemptId, token);
            var loaded = await LoadCurrentStateAsync(userId, attemptId, activeSessionId, token);
            if (loaded.State is null) return Failure<SaveCandidateExamAnswerResponse>(loaded.Error!);
            var state = loaded.State;
            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (now >= state.Part.EffectiveDeadlineAt!.Value)
                return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamPartTimeExpired);
            if (state.Question.Id != attemptQuestionId)
                return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamCurrentQuestionInvalid);

            var optionIsValid = await unitOfWork.Context.Set<QuestionRevisionOption>().AsNoTracking()
                .AnyAsync(x => x.Id == selectedOptionId &&
                               x.QuestionRevisionId == state.Question.QuestionRevisionId, token);
            if (!optionIsValid)
                return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamInvalidQuestionOption);

            if (state.Question.SelectedQuestionRevisionOptionId == selectedOptionId)
            {
                if (state.Question.SavedAt is null)
                    return Failure<SaveCandidateExamAnswerResponse>(ErrorsCodes.CandidateExamCurrentStateInvalid);
                return Result.Ok(new SaveCandidateExamAnswerResponse(state.Question.Id, selectedOptionId,
                    state.Question.SavedAt.Value, false));
            }

            state.Question.SelectedQuestionRevisionOptionId = selectedOptionId;
            state.Question.SavedAt = now;
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(new SaveCandidateExamAnswerResponse(state.Question.Id, selectedOptionId, now, true));
        }, cancellationToken);
    }

    public async Task<IResult<NavigateCandidateExamResponse>> NavigateAsync(Guid userId, Guid attemptId,
        Guid activeSessionId,
        int targetQuestionNumber, CancellationToken cancellationToken)
    {
        if (!await AttemptIsOwnedAsync(userId, attemptId, activeSessionId, cancellationToken))
            return Failure<NavigateCandidateExamResponse>(
                ErrorsCodes.CandidateExamAttemptNotFound, StatusCodes.Status404NotFound);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<NavigateCandidateExamResponse>>(async token =>
        {
            await AcquireAttemptLockAsync(attemptId, token);
            var loaded = await LoadCurrentStateAsync(userId, attemptId, activeSessionId, token);
            if (loaded.State is null) return Failure<NavigateCandidateExamResponse>(loaded.Error!);
            var state = loaded.State;
            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (now >= state.Part.EffectiveDeadlineAt!.Value)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamPartTimeExpired);

            var current = state.Part.LastQuestionNo!.Value;
            if (targetQuestionNumber == current)
                return NavigationResult(state, false);
            if (targetQuestionNumber == current + 1 && current == state.TotalQuestions)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamNoNextQuestion);
            if (targetQuestionNumber < 1 || targetQuestionNumber > state.TotalQuestions ||
                Math.Abs(targetQuestionNumber - current) != 1)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamInvalidNavigationTarget);

            if (targetQuestionNumber == current - 1 && !state.AllowPreviousQuestion)
                return Failure<NavigateCandidateExamResponse>(ErrorsCodes.CandidateExamPreviousNotAllowed);

            state.Part.LastQuestionNo = targetQuestionNumber;
            await unitOfWork.SaveChangesAsync(token);
            return NavigationResult(state, true);
        }, cancellationToken);
    }

    public async Task<IResult<SubmitCandidateExamPartResponse>> SubmitPartAsync(
        Guid userId,
        Guid attemptId,
        Guid activeSessionId,
        Guid requestedPartId,
        CancellationToken cancellationToken)
    {
        if (!await AttemptBelongsToApplicantAsync(userId, attemptId, cancellationToken))
        {
            return Failure<SubmitCandidateExamPartResponse>(
                ErrorsCodes.CandidateExamAttemptNotFound,
                StatusCodes.Status404NotFound);
        }

        return await unitOfWork.ExecuteInTransactionAsync<
            IResult<SubmitCandidateExamPartResponse>>(async token =>
        {
            await AcquireAttemptLockAsync(attemptId, token);

            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .Include(x => x.TestSessionCandidate)
                .ThenInclude(x => x!.TestSession)
                .ThenInclude(x => x!.TestSlot)
                .Include(x => x.TestSessionCandidate)
                .ThenInclude(x => x!.TestSession)
                .ThenInclude(x => x!.Exam)
                .Where(x =>
                    x.Id == attemptId &&
                    x.TestSessionCandidate!.Invitation!.ApplicantId == userId)
                .SingleOrDefaultAsync(token);

            if (attempt is null)
            {
                return Failure<SubmitCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamAttemptNotFound,
                    StatusCodes.Status404NotFound);
            }

            var activeSessionMatches = attempt.ActiveSessionId == activeSessionId;
            var terminalReplay = attempt.ActiveSessionId is null &&
                (attempt.StatusId == TestAttemptStatusIds.Completed ||
                 attempt.StatusId == TestAttemptStatusIds.TimedOut) &&
                attempt.TestSessionCandidate!.StatusId == TestSessionCandidateStatusIds.Completed;
            if (!activeSessionMatches && !terminalReplay)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamActiveSessionInvalid);
            }

            var candidate = attempt.TestSessionCandidate!;
            var session = candidate.TestSession!;
            var slot = session.TestSlot!;
            var exam = session.Exam!;

            var parts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == attemptId)
                .OrderBy(x => x.ExamPart!.PartNo)
                .Select(x => new CandidateExamPartTransitionData(
                    x,
                    x.ExamPart!.PartNo,
                    x.ExamPart.DurationMinutes,
                    x.ExamPart.QualificationScore))
                .ToListAsync(token);

            var requestedIndex = parts.FindIndex(x => x.Part.Id == requestedPartId);

            if (requestedIndex < 0)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartNotCurrent,
                    StatusCodes.Status404NotFound);
            }

            var requested = parts[requestedIndex];

            CandidateExamPartTransitionData? next = requestedIndex + 1 < parts.Count
                ? parts[requestedIndex + 1]
                : null;

            if (!activeSessionMatches)
            {
                if (requested.Part.StatusId != TestAttemptPartStatusIds.Completed)
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamActiveSessionInvalid);
                return await ExistingCompleteResult(attempt, candidate, requested, next,
                    exam.AllowPreviousQuestion, token);
            }

            // Idempotency must be checked before current attempt/session/deadline checks.
            if (requested.Part.StatusId == TestAttemptPartStatusIds.Completed)
            {
                return await ExistingCompleteResult(attempt, candidate, requested, next, exam.AllowPreviousQuestion,
                    token);
            }

            if (attempt.StatusId != TestAttemptStatusIds.InProgress)
            {
                return Failure<SubmitCandidateExamPartResponse>(AttemptStateError(attempt.StatusId));
            }

            if (candidate.StatusId != TestSessionCandidateStatusIds.Started)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamCurrentStateInvalid);
            }

            if (slot.StatusId != TestSlotStatusIds.Started)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamSlotNotRunning);
            }

            if (session.StatusId != TestSessionStatusIds.InProgress)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamSessionNotRunning);
            }

            var runningParts = parts.Where(x =>
                x.Part.StatusId == TestAttemptPartStatusIds.InProgress).ToList();

            if (runningParts.Count != 1 || runningParts[0].Part.Id != requestedPartId)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartNotCurrent);
            }

            if (!HasValidRunningState(requested.Part))
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTransitionStateInvalid);
            }

            var transitionTimeUtc = timeProvider.GetUtcNow().UtcDateTime;

            if (transitionTimeUtc >= requested.Part.EffectiveDeadlineAt!.Value)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartTimeExpired);
            }

            var currentQuestionCount = await ValidatePartQuestionOrderAsync(requested.Part.Id, token);

            if (!currentQuestionCount.HasValue)
            {
                return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamPartNotReadyToComplete);
            }

            if (requestedIndex == 0)
            {
                var score = await scoringService.CalculatePartScoreAsync(
                    attemptId,
                    requested.Part.Id,
                    token);

                if (!score.HasValue)
                {
                    return Failure<SubmitCandidateExamPartResponse>(
                        ErrorsCodes.CandidateExamConfigurationInvalid);
                }

                requested.Part.StatusId = TestAttemptPartStatusIds.Completed;
                requested.Part.CompletedAt = transitionTimeUtc;

                if (!requested.QualificationScore.HasValue)
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamConfigurationInvalid);
                }

                var passed = score.Value >= requested.QualificationScore.Value;

                if (!passed)
                {
                    requested.Part.StatusId = TestAttemptPartStatusIds.Completed;
                    requested.Part.CompletedAt = transitionTimeUtc;
                    
                    attempt.StatusId = TestAttemptStatusIds.Completed;
                    attempt.CompletedAt = transitionTimeUtc;
                    attempt.ActiveSessionId = null;
                    candidate.StatusId = TestSessionCandidateStatusIds.Completed;

                    await unitOfWork.Context
                        .Set<CandidateExamConnection>()
                        .Where(x => x.TestAttemptId == attemptId)
                        .ExecuteDeleteAsync(token);

                    await unitOfWork.SaveChangesAsync(token);

                    return CompleteResult(
                        attemptId,
                        requested,
                        CandidateExamSubmissionOutcomes.Failed,
                        null,
                        null,
                        attempt.StatusId,
                        true,
                        exam.AllowPreviousQuestion,
                        true);
                }

                if (next is null || !IsCompletelyNotStarted(next.Part))
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartStateInvalid);
                }

                if (next.DurationMinutes <= 0)
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartDurationInvalid);
                }

                var nextQuestionCount = await ValidatePartQuestionOrderAsync(next.Part.Id, token);

                if (!nextQuestionCount.HasValue)
                {
                    return Failure<SubmitCandidateExamPartResponse>(ErrorsCodes.CandidateExamNextPartQuestionsInvalid);
                }

                // Part 2 starts immediately at the same transition time.
                next.Part.StatusId =
                    TestAttemptPartStatusIds.InProgress;

                next.Part.StartedAt = transitionTimeUtc;

                next.Part.DurationMinutesSnapshot =
                    next.DurationMinutes;

                next.Part.EffectiveDeadlineAt =
                    transitionTimeUtc.AddMinutes(
                        next.DurationMinutes);

                next.Part.LastQuestionNo = 1;

                await unitOfWork.SaveChangesAsync(token);

                return CompleteResult(
                    attemptId,
                    requested,
                    CandidateExamSubmissionOutcomes.Passed,
                    next,
                    nextQuestionCount.Value,
                    attempt.StatusId,
                    false,
                    exam.AllowPreviousQuestion,
                    true);
            }

            // PART 2 must be the final part.
            if (requestedIndex != 1 || next is not null)
            {
                return Failure<SubmitCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamConfigurationInvalid);
            }

            // PART 2: no candidate-facing qualification result.
            requested.Part.StatusId = TestAttemptPartStatusIds.Completed;
            requested.Part.CompletedAt = transitionTimeUtc;

            attempt.StatusId =
                TestAttemptStatusIds.Completed;

            attempt.CompletedAt = transitionTimeUtc;
            attempt.ActiveSessionId = null;

            candidate.StatusId =
                TestSessionCandidateStatusIds.Completed;

            await unitOfWork.Context
                .Set<CandidateExamConnection>()
                .Where(x => x.TestAttemptId == attemptId)
                .ExecuteDeleteAsync(token);

            await unitOfWork.SaveChangesAsync(token);

            return CompleteResult(
                attemptId,
                requested,
                CandidateExamSubmissionOutcomes.Completed,
                null,
                null,
                attempt.StatusId,
                true,
                exam.AllowPreviousQuestion,
                true);
        }, cancellationToken);
    }

    public async Task<IResult<TimeoutCandidateExamPartResponse>> TimeoutPartAsync(
        Guid userId,
        Guid attemptId,
        Guid activeSessionId,
        Guid requestedPartId,
        CancellationToken cancellationToken)
    {
        if (!await AttemptBelongsToApplicantAsync(
                userId,
                attemptId,
                cancellationToken))
        {
            return Failure<TimeoutCandidateExamPartResponse>(
                ErrorsCodes.CandidateExamAttemptNotFound,
                StatusCodes.Status404NotFound);
        }

        return await unitOfWork.ExecuteInTransactionAsync<
            IResult<TimeoutCandidateExamPartResponse>>(async token =>
        {
            await AcquireAttemptLockAsync(attemptId, token);

            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .Where(x =>
                    x.Id == attemptId &&
                    x.TestSessionCandidate!.Invitation!.ApplicantId == userId)
                .Select(x => new
                {
                    Attempt = x,
                    x.ActiveSessionId,
                    Candidate = x.TestSessionCandidate!,
                    SessionStatusId =
                        x.TestSessionCandidate!.TestSession!.StatusId,
                    SlotStatusId =
                        x.TestSessionCandidate.TestSession.TestSlot!.StatusId
                })
                .SingleOrDefaultAsync(token);

            if (attempt is null)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamAttemptNotFound,
                    StatusCodes.Status404NotFound);
            }

            var activeSessionMatches = attempt.ActiveSessionId == activeSessionId;
            var terminalReplay = attempt.ActiveSessionId is null &&
                (attempt.Attempt.StatusId == TestAttemptStatusIds.Completed ||
                 attempt.Attempt.StatusId == TestAttemptStatusIds.TimedOut) &&
                attempt.Candidate.StatusId == TestSessionCandidateStatusIds.Completed;
            if (!activeSessionMatches && !terminalReplay)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamActiveSessionInvalid);
            }

            var parts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == attemptId)
                .OrderBy(x => x.ExamPart!.PartNo)
                .Select(x => new CandidateExamPartTransitionData(
                    x,
                    x.ExamPart!.PartNo,
                    x.ExamPart.DurationMinutes,
                    x.ExamPart.QualificationScore))
                .ToListAsync(token);

            var requestedIndex =
                parts.FindIndex(x => x.Part.Id == requestedPartId);

            if (requestedIndex < 0)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamPartNotCurrent,
                    StatusCodes.Status404NotFound);
            }

            var requested = parts[requestedIndex];

            CandidateExamPartTransitionData? next =
                requestedIndex + 1 < parts.Count
                    ? parts[requestedIndex + 1]
                    : null;

            if (!activeSessionMatches)
            {
                if (requested.Part.StatusId != TestAttemptPartStatusIds.TimedOut)
                    return Failure<TimeoutCandidateExamPartResponse>(ErrorsCodes.CandidateExamActiveSessionInvalid);
                return await ExistingTimeoutResult(attempt.Attempt, attempt.Candidate,
                    requested, next, token);
            }

            // Idempotency first.
            if (requested.Part.StatusId ==
                TestAttemptPartStatusIds.TimedOut)
            {
                return await ExistingTimeoutResult(
                    attempt.Attempt,
                    attempt.Candidate,
                    requested,
                    next,
                    token);
            }

            if (requested.Part.StatusId ==
                TestAttemptPartStatusIds.Completed)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamPartAlreadyCompleted);
            }

            if (attempt.Attempt.StatusId ==
                TestAttemptStatusIds.Interrupted ||
                requested.Part.StatusId ==
                TestAttemptPartStatusIds.Interrupted)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamAttemptInterrupted);
            }

            if (attempt.Attempt.StatusId !=
                TestAttemptStatusIds.InProgress)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    AttemptStateError(attempt.Attempt.StatusId));
            }

            if (attempt.Candidate.StatusId !=
                TestSessionCandidateStatusIds.Started)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamCurrentStateInvalid);
            }

            if (attempt.SlotStatusId != TestSlotStatusIds.Started)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamSlotNotRunning);
            }

            if (attempt.SessionStatusId !=
                TestSessionStatusIds.InProgress)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamSessionNotRunning);
            }

            var runningParts = parts
                .Where(x =>
                    x.Part.StatusId ==
                    TestAttemptPartStatusIds.InProgress)
                .ToList();

            if (runningParts.Count != 1 ||
                runningParts[0].Part.Id != requestedPartId)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamPartNotCurrent);
            }

            if (!HasValidRunningState(requested.Part))
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            var currentQuestionCount =
                await ValidatePartQuestionOrderAsync(
                    requested.Part.Id,
                    token);

            if (!currentQuestionCount.HasValue ||
                requested.Part.LastQuestionNo >
                currentQuestionCount.Value)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            var transitionTimeUtc =
                timeProvider.GetUtcNow().UtcDateTime;

            if (transitionTimeUtc <
                requested.Part.EffectiveDeadlineAt!.Value)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamPartTimeNotExpired);
            }

            // PART 1
            if (requestedIndex == 0)
            {
                var score =
                    await scoringService.CalculatePartScoreAsync(
                        attemptId,
                        requested.Part.Id,
                        token);

                if (!score.HasValue || !requested.QualificationScore.HasValue)
                {
                    return Failure<TimeoutCandidateExamPartResponse>(
                        ErrorsCodes.CandidateExamConfigurationInvalid);
                }


                var passed =
                    score.Value >=
                    requested.QualificationScore.Value;

                if (!passed)
                {
                    requested.Part.StatusId =
                        TestAttemptPartStatusIds.TimedOut;

                    requested.Part.CompletedAt =
                        requested.Part.EffectiveDeadlineAt;

                    attempt.Attempt.StatusId =
                        TestAttemptStatusIds.TimedOut;

                    attempt.Attempt.CompletedAt =
                        requested.Part.EffectiveDeadlineAt;
                    attempt.Attempt.ActiveSessionId = null;

                    attempt.Candidate.StatusId =
                        TestSessionCandidateStatusIds.Completed;

                    await unitOfWork.Context
                        .Set<CandidateExamConnection>()
                        .Where(x => x.TestAttemptId == attemptId)
                        .ExecuteDeleteAsync(token);

                    await unitOfWork.SaveChangesAsync(token);

                    return TimeoutResult(
                        attemptId,
                        requested,
                        CandidateExamSubmissionOutcomes.Failed,
                        null,
                        null,
                        attempt.Attempt.StatusId,
                        true,
                        true);
                }

                if (next is null || !IsCompletelyNotStarted(next.Part))
                {
                    return Failure<TimeoutCandidateExamPartResponse>(
                        ErrorsCodes.CandidateExamNextPartStateInvalid);
                }

                if (next.DurationMinutes <= 0)
                {
                    return Failure<TimeoutCandidateExamPartResponse>(
                        ErrorsCodes.CandidateExamNextPartDurationInvalid);
                }

                var nextQuestionCount =
                    await ValidatePartQuestionOrderAsync(
                        next.Part.Id,
                        token);

                if (!nextQuestionCount.HasValue)
                {
                    return Failure<TimeoutCandidateExamPartResponse>(
                        ErrorsCodes.CandidateExamNextPartQuestionsInvalid);
                }

                requested.Part.StatusId =
                    TestAttemptPartStatusIds.TimedOut;

                requested.Part.CompletedAt =
                    requested.Part.EffectiveDeadlineAt;

                // Part 2 starts when the timeout transition is processed.
                next.Part.StatusId =
                    TestAttemptPartStatusIds.InProgress;

                var nextPartStartedAt =
                    requested.Part.EffectiveDeadlineAt!.Value;

                next.Part.StartedAt = nextPartStartedAt;

                next.Part.DurationMinutesSnapshot =
                    next.DurationMinutes;

                next.Part.EffectiveDeadlineAt =
                    nextPartStartedAt.AddMinutes(
                        next.DurationMinutes);

                next.Part.LastQuestionNo = 1;

                await unitOfWork.SaveChangesAsync(token);

                return TimeoutResult(
                    attemptId,
                    requested,
                    CandidateExamSubmissionOutcomes.Passed,
                    next,
                    nextQuestionCount.Value,
                    attempt.Attempt.StatusId,
                    false,
                    true);
            }

            // Part 2 must be the final part.
            if (next is not null)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamConfigurationInvalid);
            }

            requested.Part.StatusId =
                TestAttemptPartStatusIds.TimedOut;

            requested.Part.CompletedAt =
                requested.Part.EffectiveDeadlineAt;

            attempt.Attempt.StatusId =
                TestAttemptStatusIds.TimedOut;

            attempt.Attempt.CompletedAt =
                requested.Part.EffectiveDeadlineAt;
            attempt.Attempt.ActiveSessionId = null;

            attempt.Candidate.StatusId =
                TestSessionCandidateStatusIds.Completed;

            await unitOfWork.Context
                .Set<CandidateExamConnection>()
                .Where(x => x.TestAttemptId == attemptId)
                .ExecuteDeleteAsync(token);

            await unitOfWork.SaveChangesAsync(token);

            return TimeoutResult(
                attemptId,
                requested,
                CandidateExamSubmissionOutcomes.Completed,
                null,
                null,
                attempt.Attempt.StatusId,
                true,
                true);
        }, cancellationToken);
    }

    private async Task<IResult<TimeoutCandidateExamPartResponse>>
        ExistingTimeoutResult(
            TestAttempt attempt,
            TestSessionCandidate candidate,
            CandidateExamPartTransitionData requested,
            CandidateExamPartTransitionData? next,
            CancellationToken token)
    {
        if (requested.Part.EffectiveDeadlineAt is null ||
            requested.Part.CompletedAt !=
            requested.Part.EffectiveDeadlineAt)
        {
            return Failure<TimeoutCandidateExamPartResponse>(
                ErrorsCodes.CandidateExamTimeoutStateInvalid);
        }

        // Final part already timed out.
        if (next is null)
        {
            if (attempt.StatusId != TestAttemptStatusIds.TimedOut ||
                attempt.CompletedAt !=
                requested.Part.EffectiveDeadlineAt ||
                candidate.StatusId !=
                TestSessionCandidateStatusIds.Completed)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            return TimeoutResult(
                attempt.Id,
                requested,
                CandidateExamSubmissionOutcomes.Completed,
                null,
                null,
                attempt.StatusId,
                true,
                false);
        }

        // Part 1 passed previously, so Part 2 was started.
        if (HasValidStartedHistory(next.Part))
        {
            var count =
                await ValidatePartQuestionOrderAsync(
                    next.Part.Id,
                    token);

            if (!count.HasValue)
            {
                return Failure<TimeoutCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamTimeoutStateInvalid);
            }

            return TimeoutResult(
                attempt.Id,
                requested,
                CandidateExamSubmissionOutcomes.Passed,
                next,
                count.Value,
                attempt.StatusId,
                attempt.StatusId != TestAttemptStatusIds.InProgress,
                false);
        }

        // Part 1 failed previously, so Part 2 never started.
        if (attempt.StatusId == TestAttemptStatusIds.TimedOut &&
            candidate.StatusId ==
            TestSessionCandidateStatusIds.Completed &&
            IsCompletelyNotStarted(next.Part))
        {
            return TimeoutResult(
                attempt.Id,
                requested,
                CandidateExamSubmissionOutcomes.Failed,
                null,
                null,
                attempt.StatusId,
                true,
                false);
        }

        return Failure<TimeoutCandidateExamPartResponse>(
            ErrorsCodes.CandidateExamTimeoutStateInvalid);
    }

    private static IResult<TimeoutCandidateExamPartResponse> TimeoutResult(
        Guid attemptId,
        CandidateExamPartTransitionData timedOut,
        string outcome,
        CandidateExamPartTransitionData? next,
        int? nextQuestionCount,
        Guid attemptStatusId,
        bool examEnded,
        bool changed)
    {
        StartedCandidateExamPartDto? nextPart = null;

        if (next is not null)
        {
            nextPart = new StartedCandidateExamPartDto(
                next.Part.Id,
                next.PartNo,
                next.Part.StartedAt!.Value,
                next.Part.DurationMinutesSnapshot!.Value,
                next.Part.EffectiveDeadlineAt!.Value,
                next.Part.LastQuestionNo!.Value,
                nextQuestionCount!.Value);
        }

        return Result.Ok(
            new TimeoutCandidateExamPartResponse(
                attemptId,
                new TimedOutCandidateExamPartDto(
                    timedOut.Part.Id,
                    timedOut.PartNo,
                    timedOut.Part.CompletedAt!.Value),
                outcome,
                next is not null,
                nextPart,
                attemptStatusId,
                examEnded,
                changed));
    }

    private async Task<int?> ValidatePartQuestionOrderAsync(Guid partId, CancellationToken token)
    {
        var order = await unitOfWork.Context.Set<TestAttemptQuestion>().AsNoTracking()
            .Where(x => x.TestAttemptPartId == partId).OrderBy(x => x.OrderNo)
            .Select(x => x.OrderNo).ToListAsync(token);
        return order.Count > 0 && order.SequenceEqual(Enumerable.Range(1, order.Count)) ? order.Count : null;
    }

    private static bool HasValidRunningState(TestAttemptPart part) =>
        part.StartedAt is not null && part.CompletedAt is null && part is
            { DurationMinutesSnapshot: > 0, EffectiveDeadlineAt: not null, LastQuestionNo: > 0 };

    private static bool IsCompletelyNotStarted(TestAttemptPart part) =>
        part.StatusId == TestAttemptPartStatusIds.NotStarted && part.StartedAt is null &&
        part.CompletedAt is null && part.DurationMinutesSnapshot is null &&
        part.EffectiveDeadlineAt is null && part.LastQuestionNo is null;

    private static bool HasValidStartedHistory(TestAttemptPart part) =>
        (part.StatusId == TestAttemptPartStatusIds.InProgress ||
         part.StatusId == TestAttemptPartStatusIds.Completed ||
         part.StatusId == TestAttemptPartStatusIds.TimedOut) &&
        part.StartedAt is not null && part.DurationMinutesSnapshot is > 0 &&
        part.EffectiveDeadlineAt >= part.StartedAt.Value.AddMinutes(part.DurationMinutesSnapshot.Value) &&
        part.LastQuestionNo is > 0 &&
        (part.StatusId == TestAttemptPartStatusIds.InProgress || part.CompletedAt is not null);

    private static IResult<SubmitCandidateExamPartResponse> CompleteResult(
        Guid attemptId,
        CandidateExamPartTransitionData completed,
        string outcome,
        CandidateExamPartTransitionData? next,
        int? nextQuestionCount,
        Guid attemptStatusId,
        bool examEnded,
        bool allowPreviousQuestion,
        bool changed)
    {
        StartedCandidateExamPartDto? nextPart = null;

        if (next is not null)
        {
            nextPart = new StartedCandidateExamPartDto(
                next.Part.Id,
                next.PartNo,
                next.Part.StartedAt!.Value,
                next.Part.DurationMinutesSnapshot!.Value,
                next.Part.EffectiveDeadlineAt!.Value,
                next.Part.LastQuestionNo!.Value,
                nextQuestionCount!.Value);
        }

        return Result.Ok(
            new SubmitCandidateExamPartResponse(
                attemptId,
                new CompletedCandidateExamPartDto(
                    completed.Part.Id,
                    completed.PartNo,
                    completed.Part.CompletedAt!.Value),
                outcome,
                next is not null,
                nextPart,
                attemptStatusId,
                examEnded,
                allowPreviousQuestion,
                changed));
    }

    private Task<bool> AttemptBelongsToApplicantAsync(
        Guid userId,
        Guid attemptId,
        CancellationToken cancellationToken) =>
        unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == attemptId &&
                x.TestSessionCandidate!.Invitation!.ApplicantId == userId,
                cancellationToken);

    private Task<bool> AttemptIsOwnedAsync(
        Guid userId,
        Guid attemptId,
        Guid activeSessionId,
        CancellationToken cancellationToken) =>
        unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .AnyAsync(x =>
                    x.Id == attemptId &&
                    x.TestSessionCandidate!.Invitation!.ApplicantId == userId &&
                    x.ActiveSessionId == activeSessionId,
                cancellationToken);

    private async Task<(CandidateExamExecutionState? State, string? Error)> LoadCurrentStateAsync(
        Guid userId,
        Guid attemptId,
        Guid activeSessionId,
        CancellationToken token)
    {
        var attempt = await unitOfWork.Context.Set<TestAttempt>().AsNoTracking()
            .Where(x => x.Id == attemptId && x.TestSessionCandidate!.Invitation!.ApplicantId == userId)
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
            .SingleOrDefaultAsync(token);
        if (attempt is null) return (null, ErrorsCodes.CandidateExamAttemptNotFound);
        if (attempt.StatusId != TestAttemptStatusIds.InProgress)
            return (null, AttemptStateError(attempt.StatusId));
        if (attempt.CandidateStatusId != TestSessionCandidateStatusIds.Started)
            return (null, ErrorsCodes.CandidateExamCurrentStateInvalid);
        if (attempt.SlotStatusId != TestSlotStatusIds.Started)
            return (null, ErrorsCodes.CandidateExamSlotNotRunning);
        if (attempt.SessionStatusId != TestSessionStatusIds.InProgress)
            return (null, ErrorsCodes.CandidateExamSessionNotRunning);
        if (attempt.ActiveSessionId != activeSessionId)
            return (null, ErrorsCodes.CandidateExamActiveSessionInvalid);

        var runningParts = await unitOfWork.Context.Set<TestAttemptPart>()
            .Where(x => x.TestAttemptId == attemptId && x.StatusId == TestAttemptPartStatusIds.InProgress)
            .Take(2).ToListAsync(token);
        if (runningParts.Count != 1) return (null, ErrorsCodes.CandidateExamCurrentStateInvalid);
        var part = runningParts[0];
        if (part.StartedAt is null || part.DurationMinutesSnapshot is not > 0 ||
            part.EffectiveDeadlineAt is null || part.LastQuestionNo is not > 0)
            return (null, ErrorsCodes.CandidateExamCurrentStateInvalid);

        var total = await unitOfWork.Context.Set<TestAttemptQuestion>().AsNoTracking()
            .CountAsync(x => x.TestAttemptPartId == part.Id, token);
        if (total == 0 || part.LastQuestionNo > total)
            return (null, ErrorsCodes.CandidateExamCurrentQuestionInvalid);
        var currentQuestions = await unitOfWork.Context.Set<TestAttemptQuestion>()
            .Where(x => x.TestAttemptId == attemptId && x.TestAttemptPartId == part.Id &&
                        x.OrderNo == part.LastQuestionNo.Value).Take(2).ToListAsync(token);
        if (currentQuestions.Count != 1)
            return (null, ErrorsCodes.CandidateExamCurrentQuestionInvalid);

        return (new CandidateExamExecutionState(attempt.Id, part, currentQuestions[0], total,
            attempt.AllowPreviousQuestion), null);
    }

    private async Task<IResult<SubmitCandidateExamPartResponse>>
        ExistingCompleteResult(
            TestAttempt attempt,
            TestSessionCandidate candidate,
            CandidateExamPartTransitionData requested,
            CandidateExamPartTransitionData? next,
            bool allowPreviousQuestion,
            CancellationToken token)
    {
        if (requested.Part.CompletedAt is null)
        {
            return Failure<SubmitCandidateExamPartResponse>(
                ErrorsCodes.CandidateExamPartTransitionStateInvalid);
        }

        // Final part was already submitted successfully.
        if (next is null)
        {
            if (attempt.StatusId != TestAttemptStatusIds.Completed ||
                candidate.StatusId !=
                TestSessionCandidateStatusIds.Completed)
            {
                return Failure<SubmitCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamPartTransitionStateInvalid);
            }

            return CompleteResult(
                attempt.Id,
                requested,
                CandidateExamSubmissionOutcomes.Completed,
                null,
                null,
                attempt.StatusId,
                true,
                allowPreviousQuestion,
                false);
        }

        // Part 1 passed previously: Part 2 has started.
        if (HasValidStartedHistory(next.Part))
        {
            var nextQuestionCount =
                await ValidatePartQuestionOrderAsync(
                    next.Part.Id,
                    token);

            if (!nextQuestionCount.HasValue)
            {
                return Failure<SubmitCandidateExamPartResponse>(
                    ErrorsCodes.CandidateExamPartTransitionStateInvalid);
            }

            return CompleteResult(
                attempt.Id,
                requested,
                CandidateExamSubmissionOutcomes.Passed,
                next,
                nextQuestionCount.Value,
                attempt.StatusId,
                attempt.StatusId != TestAttemptStatusIds.InProgress,
                allowPreviousQuestion,
                false);
        }

        // Part 1 failed previously: Part 2 never started.
        if (attempt.StatusId == TestAttemptStatusIds.Completed &&
            candidate.StatusId ==
            TestSessionCandidateStatusIds.Completed &&
            IsCompletelyNotStarted(next.Part))
        {
            return CompleteResult(
                attempt.Id,
                requested,
                CandidateExamSubmissionOutcomes.Failed,
                null,
                null,
                attempt.StatusId,
                true,
                allowPreviousQuestion,
                false);
        }

        return Failure<SubmitCandidateExamPartResponse>(
            ErrorsCodes.CandidateExamPartTransitionStateInvalid);
    }

    private Task AcquireAttemptLockAsync(Guid attemptId, CancellationToken token) =>
        unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
            "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
            "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam execution lock unavailable', 1;",
            [$"Tawtheef.CandidateExam.Attempt.{attemptId}"], token);

    private static IResult<NavigateCandidateExamResponse> NavigationResult(
        CandidateExamExecutionState state, bool changed)
    {
        var current = state.Part.LastQuestionNo!.Value;
        return Result.Ok(new NavigateCandidateExamResponse(state.AttemptId, state.Part.Id, current,
            state.TotalQuestions, state.AllowPreviousQuestion && current > 1,
            current < state.TotalQuestions, state.AllowPreviousQuestion, changed));
    }

    private static string AttemptStateError(Guid statusId) =>
        statusId == TestAttemptStatusIds.NotStarted ? ErrorsCodes.CandidateExamAttemptNotStarted :
        statusId == TestAttemptStatusIds.Interrupted ? ErrorsCodes.CandidateExamAttemptInterrupted :
        statusId == TestAttemptStatusIds.Completed ? ErrorsCodes.CandidateExamAttemptCompleted :
        statusId == TestAttemptStatusIds.TimedOut ? ErrorsCodes.CandidateExamAttemptTimedOut :
        statusId == TestAttemptStatusIds.Cancelled ? ErrorsCodes.CandidateExamAttemptCancelled :
        ErrorsCodes.CandidateExamCurrentStateInvalid;

    private static IResult<T> Failure<T>(string code, int statusCode = 409) =>
        Result.Fail<T>(new Error(code).WithMetadata("Code", code).WithMetadata("StatusCode", statusCode));
}
