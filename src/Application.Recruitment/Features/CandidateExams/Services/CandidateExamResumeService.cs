using Application.Recruitment.Features.CandidateExams.Models;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamResumeService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    CandidateExamEntryValidationService entryValidation)
{
    public Task<CandidateExamResumeResult?> ResumeAsync(
        Guid userId,
        Guid attemptId,
        string accessCode,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<CandidateExamResumeResult?>(async token =>
        {
            await AcquireAttemptLockAsync(attemptId, token);

            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .Where(x => x.Id == attemptId && x.ActiveSessionId == null &&
                    x.TestSessionCandidate!.Invitation!.ApplicantId == userId &&
                    x.TestSessionCandidate.StatusId == TestSessionCandidateStatusIds.Started &&
                    x.TestSessionCandidate.TestSession!.StatusId == TestSessionStatusIds.InProgress &&
                    x.TestSessionCandidate.TestSession.TestSlot!.StatusId == TestSlotStatusIds.Started &&
                    (x.TestSessionCandidate.TestSession.Exam!.InterruptionPolicyId ==
                     ExamInterruptionPolicyIds.ResumeOnly ||
                     x.TestSessionCandidate.TestSession.Exam.InterruptionPolicyId ==
                     ExamInterruptionPolicyIds.ResumeOrReschedule))
                .SingleOrDefaultAsync(token);
            if (attempt is null)
                return null;
            
            var entry = await entryValidation.ValidateForResumeAsync(
                userId,
                attempt.TestSessionCandidateId,
                accessCode,
                token);

            if (entry.IsFailed)
                return null;

            var currentParts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == attemptId &&
                    (x.StatusId == TestAttemptPartStatusIds.Interrupted ||
                     x.StatusId == TestAttemptPartStatusIds.InProgress))
                .Select(x => new
                {
                    Part = x,
                    x.TestAttempt!.TestSessionCandidate!.TestSession!.Exam!.AllowPreviousQuestion
                })
                .Take(2)
                .ToListAsync(token);

            if (attempt.StatusId != TestAttemptStatusIds.Interrupted ||
                currentParts.Count != 1 ||
                currentParts[0].Part.StatusId != TestAttemptPartStatusIds.Interrupted ||
                currentParts[0].Part.EffectiveDeadlineAt is null ||
                currentParts[0].Part.LastQuestionNo is not > 0)
                return null;

            var currentPart = currentParts[0];
            var part = currentPart.Part;
            if (part.EffectiveDeadlineAt is not { } oldDeadline)
                return null;

            var interruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .Where(x => x.TestAttemptId == attemptId &&
                    x.TestAttemptPartId == part.Id &&
                    x.StatusId == TestAttemptInterruptionStatusIds.Open)
                .Take(2)
                .ToListAsync(token);
            if (interruptions.Count != 1)
                return null;

            var interruption = interruptions[0];
            
            if (interruption.ResolutionActionId !=
                TestAttemptInterruptionResolutionActionIds.Resume ||
                interruption.ResolvedById is null ||
                interruption.ResolvedAt is not null)
            {
                return null;
            }
            
            var resumeAt = timeProvider.GetUtcNow().UtcDateTime;
            
            var compensatedDuration = resumeAt - interruption.InterruptedAt;
            if (compensatedDuration < TimeSpan.Zero)
                return null;

            var newDeadline = oldDeadline.Add(compensatedDuration);
            var newActiveSessionId = Guid.NewGuid();
            part.EffectiveDeadlineAt = newDeadline;
            part.StatusId = TestAttemptPartStatusIds.InProgress;
            attempt.StatusId = TestAttemptStatusIds.InProgress;
            attempt.ActiveSessionId = newActiveSessionId;
            interruption.StatusId = TestAttemptInterruptionStatusIds.Resolved;
            interruption.ResolvedAt = resumeAt;

            await unitOfWork.SaveChangesAsync(token);

            await unitOfWork.Context.Set<CandidateExamConnection>()
                .Where(x => x.TestAttemptId == attemptId)
                .ExecuteDeleteAsync(token);

            return new CandidateExamResumeResult(
                attemptId,
                part.Id,
                newActiveSessionId,
                newDeadline,
                part.LastQuestionNo!.Value,
                currentPart.AllowPreviousQuestion,
                true);
        }, cancellationToken);

    private Task AcquireAttemptLockAsync(Guid attemptId, CancellationToken cancellationToken) =>
        unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
            "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
            "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam resume lock unavailable', 1;",
            [$"Tawtheef.CandidateExam.Attempt.{attemptId}"], cancellationToken);
}
