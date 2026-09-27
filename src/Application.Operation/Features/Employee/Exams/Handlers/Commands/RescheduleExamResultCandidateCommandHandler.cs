using System.Text.Json;
using Application.Operation.Features.Employee.Exams.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class RescheduleExamResultCandidateCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    TimeProvider timeProvider)
    : IRequestHandler<RescheduleExamResultCandidateCommand, IResult<Guid>>
{
    public Task<IResult<Guid>> Handle(
        RescheduleExamResultCandidateCommand request,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(async token =>
        {
            if (!Guid.TryParse(currentUser.UserId, out var employeeId) ||
                string.IsNullOrWhiteSpace(request.Reason))
                return Result.Fail<Guid>(ErrorsCodes.InvalidRequest);

            var reason = request.Reason.Trim();
            await AcquireLockAsync(
                $"Tawtheef.ExamResultReport.Approve.{request.ReportId}", token);

            var report = await unitOfWork.Context.Set<ExamResultReport>()
                .Include(x => x.Candidates)
                .FirstOrDefaultAsync(x => x.Id == request.ReportId, token);
            if (report is null)
                return Failure(ErrorsCodes.ExamResultReportNotFound);

            var candidate = report.Candidates.FirstOrDefault(x => x.Id == request.CandidateResultId);
            if (candidate is null)
                return Failure(ErrorsCodes.ExamResultCandidateNotFound);
            if (report.StatusId != ExamResultReportStatusIds.Returned)
                return Failure(ErrorsCodes.ExamResultReportNotReturned);
            if (candidate.ResultStatusId != ExamResultCandidateResultStatusIds.RequiresAction)
                return Failure(ErrorsCodes.ExamResultCandidateNotRequiresAction);
            if (candidate.TestSessionCandidateId is not Guid currentAssignmentId)
                return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);

            var currentAssignment = await unitOfWork.Context.Set<TestSessionCandidate>()
                .Include(x => x.TestSession)
                .ThenInclude(x => x!.TestSlot)
                .Include(x => x.TestAttempt)
                .SingleOrDefaultAsync(x => x.Id == currentAssignmentId, token);
            if (currentAssignment is null || currentAssignment.InvitationId != candidate.InvitationId ||
                currentAssignment.TestSession?.ExamId != report.ExamId ||
                currentAssignment.TestAttempt?.Id != candidate.TestAttemptId)
                return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);

            if (await unitOfWork.Context.Set<TestSessionCandidate>()
                    .AsNoTracking()
                    .AnyAsync(x => x.RescheduledFromTestSessionCandidateId == currentAssignmentId, token))
                return Failure(ErrorsCodes.ExamResultCandidateAlreadyRescheduled);

            var targetSession = await unitOfWork.Context.Set<TestSession>()
                .AsNoTracking()
                .Include(x => x.TestSlot)
                .ThenInclude(x => x!.Room)
                .SingleOrDefaultAsync(x => x.Id == request.TargetTestSessionId, token);
            if (targetSession is null || targetSession.Id == currentAssignment.TestSessionId ||
                targetSession.ExamId != report.ExamId ||
                (targetSession.StatusId != TestSessionStatusIds.Approved &&
                 targetSession.StatusId != TestSessionStatusIds.Ready) ||
                targetSession.TestSlot is not { StatusId: var slotStatus, Room: { } room } ||
                slotStatus != TestSlotStatusIds.Ready || room.StatusId != RoomStatusIds.Active ||
                room.Capacity <= 0)
                return Failure(ErrorsCodes.ExamResultCandidateTargetSessionInvalid);

            var targetSlot = targetSession.TestSlot;
            if (targetSlot.SlotDate.ToDateTime(targetSlot.StartTime) <=
                timeProvider.GetLocalNow().DateTime)
                return Failure(ErrorsCodes.ExamResultCandidateTargetSlotNotFuture);

            // Capacity is shared by all sessions in the same physical room-time slot.
            await AcquireLockAsync($"Tawtheef.CandidateExam.TestSlot.{targetSlot.Id}", token);

            var activeCandidateCount = await unitOfWork.Context.Set<TestSessionCandidate>()
                .AsNoTracking()
                .CountAsync(x => x.TestSession!.TestSlotId == targetSlot.Id &&
                    x.StatusId != TestSessionCandidateStatusIds.Cancelled &&
                    x.StatusId != TestSessionCandidateStatusIds.Rescheduled, token);
            if (activeCandidateCount >= room.Capacity)
                return Failure(ErrorsCodes.ExamResultCandidateTargetSlotFull);

            if (await unitOfWork.Context.Set<TestSessionCandidate>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id != currentAssignmentId &&
                        x.InvitationId == candidate.InvitationId &&
                        x.TestSession!.ExamId == report.ExamId &&
                        x.StatusId != TestSessionCandidateStatusIds.Cancelled &&
                        x.StatusId != TestSessionCandidateStatusIds.Rescheduled, token))
                return Failure(ErrorsCodes.ExamResultCandidateActiveAssignmentExists);

            var rescheduledAt = timeProvider.GetUtcNow().UtcDateTime;
            var previousAttemptId = candidate.TestAttemptId;
            Guid? previousAttemptStatus = null;
            Guid? interruptionId = null;

            if (candidate.TestAttemptId is Guid attemptId)
            {
                await AcquireLockAsync($"Tawtheef.CandidateExam.Attempt.{attemptId}", token);
                var attempt = await unitOfWork.Context.Set<TestAttempt>()
                    .SingleOrDefaultAsync(x => x.Id == attemptId &&
                        x.TestSessionCandidateId == currentAssignmentId, token);
                if (attempt is null)
                    return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);

                previousAttemptStatus = attempt.StatusId;
                var openInterruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                    .Where(x => x.TestAttemptId == attemptId &&
                        x.StatusId == TestAttemptInterruptionStatusIds.Open)
                    .Take(2)
                    .ToListAsync(token);
                if (openInterruptions.Count != 1 || openInterruptions[0].ResolvedAt is not null)
                    return Failure(ErrorsCodes.ExamResultCandidateInterruptionStateInvalid);

                if (attempt.StatusId == TestAttemptStatusIds.Interrupted)
                {
                    if (attempt.ActiveSessionId is not null ||
                        openInterruptions[0].TestAttemptPartId is not Guid interruptedPartId ||
                        !await unitOfWork.Context.Set<TestAttemptPart>().AnyAsync(x =>
                            x.Id == interruptedPartId && x.TestAttemptId == attemptId &&
                            x.StatusId == TestAttemptPartStatusIds.Interrupted, token))
                        return Failure(ErrorsCodes.ExamResultCandidateInterruptionStateInvalid);

                    attempt.StatusId = TestAttemptStatusIds.Cancelled;
                    attempt.ActiveSessionId = null;
                }
                else if (attempt.StatusId != TestAttemptStatusIds.Completed &&
                         attempt.StatusId != TestAttemptStatusIds.TimedOut)
                {
                    return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);
                }

                var interruption = openInterruptions[0];
                interruption.ResolutionActionId = TestAttemptInterruptionResolutionActionIds.Reschedule;
                interruption.ResolutionNotes = reason;
                interruption.ResolvedById = employeeId;
                interruption.ResolvedAt = rescheduledAt;
                interruption.StatusId = TestAttemptInterruptionStatusIds.Resolved;
                interruptionId = interruption.Id;
            }

            var transition = currentAssignment.MarkRescheduled(reason);
            if (transition.IsFailed)
                return Result.Fail<Guid>(transition.Errors);

            var replacement = new TestSessionCandidate
            {
                Id = Guid.NewGuid(),
                TestSessionId = targetSession.Id,
                InvitationId = currentAssignment.InvitationId,
                AttendanceStatusId = TestSessionCandidateAttendanceStatusIds.Pending,
                IdentityVerificationStatusId = TestSessionCandidateIdentityVerificationStatusIds.Pending,
                IdentityVerificationNotes = null,
                AuthorizedAt = null,
                StatusId = TestSessionCandidateStatusIds.Assigned,
                RescheduleReason = null,
                RescheduledFromTestSessionCandidateId = currentAssignmentId
            };
            await unitOfWork.Context.Set<TestSessionCandidate>().AddAsync(replacement, token);

            candidate.TestSessionCandidateId = replacement.Id;
            candidate.TestSessionCandidate = replacement;
            candidate.TestAttemptId = null;
            candidate.TestAttempt = null;
            candidate.SpecializedScore = null;
            candidate.EducationalScore = null;
            candidate.SkillsScore = null;
            candidate.FinalScore = null;
            candidate.Rank = null;
            candidate.SnapshotAt = rescheduledAt;

            await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(new ActionLog
            {
                UserId = employeeId,
                LogType = ActionLogType.Employee,
                ActionType = "ExamResultCandidateRescheduled",
                Section = "ExamResultReport",
                EntityId = report.Id,
                Notes = JsonSerializer.Serialize(new
                {
                    reportId = report.Id,
                    examId = report.ExamId,
                    candidateResultId = candidate.Id,
                    invitationId = candidate.InvitationId,
                    oldTestSessionCandidateId = currentAssignmentId,
                    newTestSessionCandidateId = replacement.Id,
                    oldTestSessionId = currentAssignment.TestSessionId,
                    newTestSessionId = targetSession.Id,
                    oldTestAttemptId = previousAttemptId,
                    oldAttemptStatus = previousAttemptStatus,
                    interruptionId,
                    interruptionResolutionAction = interruptionId.HasValue
                        ? TestAttemptInterruptionResolutionActionIds.Reschedule
                        : (Guid?)null,
                    employeeId,
                    reason,
                    rescheduledAt,
                    reportStatus = report.StatusId,
                    resultStatus = candidate.ResultStatusId
                })
            }, token);

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(replacement.Id);
        }, cancellationToken);

    private Task AcquireLockAsync(string resource, CancellationToken cancellationToken) =>
        unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
            "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
            "@LockTimeout = 15000; IF @result < 0 " +
            "THROW 51000, 'Candidate exam reschedule lock unavailable', 1;",
            [resource], cancellationToken);

    private static IResult<Guid> Failure(string code) => Result.Fail<Guid>(new Error(code));
}
