using Application.Recruitment.Features.CandidateExams.Common;
using Application.Recruitment.Features.CandidateExams.Models;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamResumeService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    CandidateExamEntryValidationService entryValidation,
    IApplicationLockService applicationLockService)
{
    public Task<CandidateExamResumeResult?> ResumeAsync(Guid userId, Guid attemptId, string accessCode,
        Guid activeSessionId, CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<CandidateExamResumeResult?>(async token =>
        {
            var testSlotId = await unitOfWork.Context.Set<TestAttempt>()
                .OwnedByApplicant(userId)
                .Where(x => x.Id == attemptId)
                .Select(x => x.TestSessionCandidate!.TestSession!.TestSlotId)
                .SingleOrDefaultAsync(token);

            if (testSlotId is null)
                return null;

            // Lock order must stay Slot -> Attempt.
            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamTestSlot(testSlotId.Value), token);

            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);


            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .OwnedByApplicant(userId)
                .Where(x => x.Id == attemptId && x.TestSessionCandidate!.TestSession!.TestSlotId == testSlotId.Value
                                              && x.TestSessionCandidate.StatusId
                                              == TestSessionCandidateStatusIds.Started
                                              && x.TestSessionCandidate.TestSession!.StatusId
                                              == TestSessionStatusIds.InProgress
                                              && x.TestSessionCandidate.TestSession.TestSlot!.StatusId
                                              == TestSlotStatusIds.Started
                                              && (x.TestSessionCandidate.TestSession.Exam!.InterruptionPolicyId
                                                  == ExamInterruptionPolicyIds.ResumeOnly
                                                  || x.TestSessionCandidate.TestSession.Exam.InterruptionPolicyId
                                                  == ExamInterruptionPolicyIds.ResumeOrReschedule))
                .SingleOrDefaultAsync(token);
            if (attempt is null)
                return null;

            var entry = await entryValidation.ValidateForResumeAsync(userId, attempt.TestSessionCandidateId, accessCode,
                token);

            if (entry.IsFailed)
                return null;

            var currentParts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == attemptId && (x.StatusId == TestAttemptPartStatusIds.Interrupted
                                                             || x.StatusId == TestAttemptPartStatusIds.InProgress))
                .Select(x => new
                {
                    Part = x, x.TestAttempt!.TestSessionCandidate!.TestSession!.Exam!.AllowPreviousQuestion
                })
                .Take(2)
                .ToListAsync(token);


            if (attempt.StatusId == TestAttemptStatusIds.InProgress && attempt.ActiveSessionId == activeSessionId)
            {
                if (currentParts.Count != 1 || currentParts[0].Part.StatusId != TestAttemptPartStatusIds.InProgress
                                            || currentParts[0].Part.EffectiveDeadlineAt is null
                                            || currentParts[0].Part.LastQuestionNo is not > 0)
                {
                    return null;
                }

                var currPart = currentParts[0];

                var wasResumed = await unitOfWork.Context.Set<TestAttemptInterruption>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.TestAttemptId == attemptId && x.TestAttemptPartId == currPart.Part.Id
                                                          && x.StatusId == TestAttemptInterruptionStatusIds.Resolved
                                                          && x.ResolutionActionId
                                                          == TestAttemptInterruptionResolutionActionIds.Resume
                                                          && x.ResolvedAt.HasValue, token);

                if (!wasResumed)
                    return null;

                return new CandidateExamResumeResult(attemptId, currPart.Part.Id, activeSessionId,
                    currPart.Part.EffectiveDeadlineAt!.Value.AsUtcOffset(), currPart.Part.LastQuestionNo!.Value,
                    currPart.AllowPreviousQuestion, false);
            }

            if (attempt.StatusId != TestAttemptStatusIds.Interrupted || attempt.ActiveSessionId is not null
                                                                     || currentParts.Count != 1
                                                                     || currentParts[0].Part.StatusId
                                                                     != TestAttemptPartStatusIds.Interrupted
                                                                     || currentParts[0].Part.EffectiveDeadlineAt is null
                                                                     || currentParts[0].Part.LastQuestionNo is not > 0)
                return null;

            var currentPart = currentParts[0];
            var part = currentPart.Part;
            if (part.EffectiveDeadlineAt is not { } oldDeadline)
                return null;

            var interruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .Where(x => x.TestAttemptId == attemptId && x.TestAttemptPartId == part.Id
                                                         && x.StatusId == TestAttemptInterruptionStatusIds.Open)
                .Take(2)
                .ToListAsync(token);
            if (interruptions.Count != 1)
                return null;

            var interruption = interruptions[0];

            if (interruption.ResolutionActionId != TestAttemptInterruptionResolutionActionIds.Resume
                || interruption.ResolvedById is null || interruption.ResolvedAt is not null)
            {
                return null;
            }

            var resumeAt = timeProvider.GetUtcNow().UtcDateTime;

            var compensatedDuration = resumeAt - interruption.InterruptedAt;
            if (compensatedDuration < TimeSpan.Zero)
                return null;

            var newDeadline = oldDeadline.Add(compensatedDuration);

            part.Resume(newDeadline);
            attempt.Resume(activeSessionId);
            interruption.StatusId = TestAttemptInterruptionStatusIds.Resolved;
            interruption.ResolvedAt = resumeAt;

            await unitOfWork.SaveChangesAsync(token);

            await unitOfWork.Context.Set<CandidateExamConnection>()
                .Where(x => x.TestAttemptId == attemptId)
                .ExecuteDeleteAsync(token);

            return new CandidateExamResumeResult(attemptId, part.Id, activeSessionId, newDeadline.AsUtcOffset(),
                part.LastQuestionNo!.Value, currentPart.AllowPreviousQuestion, true);
        }, cancellationToken);
}
