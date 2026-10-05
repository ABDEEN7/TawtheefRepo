using System.Text.Json;
using Application.Operation.Features.Employee.CandidateExamAttempts.Commands;
using Application.Operation.Features.Employee.TestSessions.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.Operation.Employee.Exams;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Handlers.Commands;

public sealed class RescheduleInterruptedCandidateExamCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
    : IRequestHandler<RescheduleInterruptedCandidateExamCommand, IResult<Guid>>
{
    public Task<IResult<Guid>> Handle(RescheduleInterruptedCandidateExamCommand request,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(async token =>
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return Result.Fail<Guid>(ErrorsCodes.InvalidRequest);

            var reason = request.Reason.Trim();

            var preliminary = await unitOfWork.Context.Set<TestAttempt>()
                .AsNoTracking()
                .Where(x => x.Id == request.TestAttemptId)
                .Select(x => new
                {
                    SourceSlotId = x.TestSessionCandidate!.TestSession!.TestSlotId,
                    AssignmentId = x.TestSessionCandidateId,
                    x.TestSessionCandidate!.InvitationId,
                    x.TestSessionCandidate!.TestSession!.ExamId,
                    CurrentSessionId = x.TestSessionCandidate!.TestSessionId
                })
                .SingleOrDefaultAsync(token);

            if (preliminary is null)
                return Unavailable();

            var preliminaryTarget = await unitOfWork.Context.Set<TestSession>()
                .AsNoTracking()
                .Where(x => x.Id == request.TargetTestSessionId)
                .Select(x => new { x.TestSlotId, x.TestSlot!.RoomId, x.TestSlot.SlotDate })
                .SingleOrDefaultAsync(token);

            if (preliminaryTarget is null)
                return TargetInvalid();


            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.TestSessionRoomDate(preliminaryTarget.RoomId, preliminaryTarget.SlotDate),
                token);

            var slotIds = new[] { preliminary.SourceSlotId, preliminaryTarget.TestSlotId }.Distinct()
                .OrderBy(x => x)
                .ToArray();

            foreach (var slotId in slotIds)
            {
                await applicationLockService.AcquireExclusiveAsync(
                    ApplicationLockResources.CandidateExamTestSlot(slotId), token);
            }

            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamAttempt(request.TestAttemptId), token);

            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .Include(x => x.TestSessionCandidate)
                .ThenInclude(x => x!.TestSession)
                .ThenInclude(x => x!.TestSlot)
                .Include(x => x.TestSessionCandidate)
                .ThenInclude(x => x!.TestSession)
                .ThenInclude(x => x!.Exam)
                .SingleOrDefaultAsync(x => x.Id == request.TestAttemptId, token);

            if (attempt?.TestSessionCandidate is not { } currentAssignment
                || currentAssignment.TestSession is not { } currentSession
                || currentSession.TestSlot is not { } sourceSlot || currentSession.Exam is not { } exam
                || currentAssignment.Id != preliminary.AssignmentId
                || currentAssignment.InvitationId != preliminary.InvitationId
                || currentSession.Id != preliminary.CurrentSessionId || currentSession.ExamId != preliminary.ExamId
                || sourceSlot.Id != preliminary.SourceSlotId || attempt.StatusId != TestAttemptStatusIds.Interrupted
                || attempt.ActiveSessionId is not null
                || currentAssignment.StatusId != TestSessionCandidateStatusIds.Started
                || currentSession.StatusId != TestSessionStatusIds.InProgress
                || sourceSlot.StatusId != TestSlotStatusIds.Started
                || (exam.InterruptionPolicyId != ExamInterruptionPolicyIds.RescheduleOnly
                    && exam.InterruptionPolicyId != ExamInterruptionPolicyIds.ResumeOrReschedule))
            {
                return Unavailable();
            }

            var isHallSupervisor = await unitOfWork.Context.Set<TestSlotStaff>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.TestSlotId == sourceSlot.Id && x.StaffUserId == request.OfficerId && x.IsActive
                         && x.RoleId == TestSlotStaffRoleIds.HallSupervisor, token);

            if (!isHallSupervisor)
            {
                return Result.Fail<Guid>(
                    new Error(ErrorsCodes.UnauthorizedAction).WithMetadata("StatusCode",
                        StatusCodes.Status403Forbidden));
            }

            var interruptedParts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == request.TestAttemptId
                            && x.StatusId == TestAttemptPartStatusIds.Interrupted)
                .Take(2)
                .ToListAsync(token);

            if (interruptedParts.Count != 1)
                return Unavailable();

            var interruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .Where(x => x.TestAttemptId == request.TestAttemptId && x.TestAttemptPartId == interruptedParts[0].Id
                                                                     && x.StatusId == TestAttemptInterruptionStatusIds
                                                                         .Open)
                .Take(2)
                .ToListAsync(token);

            if (interruptions.Count != 1)
                return Unavailable();

            var interruption = interruptions[0];

            if (interruption.ResolutionActionId is not null || interruption.ResolvedById is not null
                                                            || interruption.ResolvedAt is not null)
            {
                return Unavailable();
            }

            var alreadyRescheduled = await unitOfWork.Context.Set<TestSessionCandidate>()
                .AsNoTracking()
                .AnyAsync(x => x.RescheduledFromTestSessionCandidateId == currentAssignment.Id, token);

            if (alreadyRescheduled)
            {
                return Result.Fail<Guid>(ErrorsCodes.CandidateExamAlreadyRescheduled);
            }

            var targetSession = await unitOfWork.Context.Set<TestSession>()
                .AsNoTracking()
                .Include(x => x.TestSlot)
                .ThenInclude(x => x!.Room)
                .ThenInclude(x => x!.Location)
                .SingleOrDefaultAsync(x => x.Id == request.TargetTestSessionId, token);

            if (targetSession is null || targetSession.Id == currentSession.Id
                                      || targetSession.ExamId != currentSession.ExamId
                                      || targetSession.StartTime is null || targetSession.EndTime is null
                                      || targetSession.StartTime >= targetSession.EndTime
                                      || (targetSession.StatusId != TestSessionStatusIds.Approved
                                          && targetSession.StatusId != TestSessionStatusIds.Ready)
                                      || targetSession.TestSlot is not
                                      {
                                          Room: { } room
                                      } targetSlot
                                      || (targetSlot.StatusId != TestSlotStatusIds.Ready
                                          && targetSlot.StatusId != TestSlotStatusIds.Started)
                                      || room.StatusId != RoomStatusIds.Active || room.Capacity <= 0)
            {
                return TargetInvalid();
            }

            if (targetSlot.Id != preliminaryTarget.TestSlotId || targetSlot.RoomId != preliminaryTarget.RoomId
                                                              || targetSlot.SlotDate != preliminaryTarget.SlotDate)
            {
                return TargetInvalid();
            }

            var targetStart = targetSession.StartTime.Value;

            var targetEnd = targetSession.EndTime.Value;

            if (targetStart < targetSlot.StartTime || targetEnd > targetSlot.EndTime)
            {
                return TargetInvalid();
            }

            var localNow = timeProvider.GetLocalNow().DateTime;

            var targetStartsAt = targetSlot.SlotDate.ToDateTime(targetStart);

            if (targetStartsAt <= localNow)
            {
                return Result.Fail<Guid>(ErrorsCodes.CandidateExamRescheduleTargetSlotNotFuture);
            }

            var reservations = await unitOfWork.Context.Set<TestSession>()
                .AsNoTracking()
                .Where(existing => existing.TestSlot != null && existing.TestSlot.RoomId == targetSlot.RoomId
                                                             && existing.TestSlot.SlotDate == targetSlot.SlotDate
                                                             && existing.StartTime.HasValue && existing.EndTime.HasValue
                                                             && !TestSessionCapacityService.NonReservingStatusIds
                                                                 .Contains(existing.StatusId))
                .Select(existing => new TestSessionCapacityReservation(existing.StartTime!.Value,
                    existing.EndTime!.Value,
                    unitOfWork.Context.Set<TestSessionCandidate>()
                        .Count(assignment => assignment.TestSessionId == existing.Id
                                             && assignment.Id != currentAssignment.Id
                                             && assignment.StatusId != TestSessionCandidateStatusIds.Cancelled
                                             && assignment.StatusId != TestSessionCandidateStatusIds.Rescheduled)))
                .ToListAsync(token);

            var occupiedSeats = TestSessionCapacityService.CalculatePeakOccupancy(reservations, targetStart, targetEnd);

            if (occupiedSeats >= room.Capacity)
            {
                return Result.Fail<Guid>(ErrorsCodes.CandidateExamRescheduleTargetSlotFull);
            }

            var activeAssignmentExists = await unitOfWork.Context.Set<TestSessionCandidate>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id != currentAssignment.Id && x.InvitationId == currentAssignment.InvitationId
                                                      && x.TestSession!.ExamId == currentSession.ExamId
                                                      && x.StatusId != TestSessionCandidateStatusIds.Cancelled
                                                      && x.StatusId != TestSessionCandidateStatusIds.Rescheduled,
                    token);

            if (activeAssignmentExists)
                return Unavailable();

            var rescheduledAt = timeProvider.GetUtcNow().UtcDateTime;


            attempt.CancelInterrupted();
            interruption.ResolutionActionId = TestAttemptInterruptionResolutionActionIds.Reschedule;
            interruption.ResolutionNotes = reason;
            interruption.ResolvedById = request.OfficerId;
            interruption.ResolvedAt = rescheduledAt;
            interruption.StatusId = TestAttemptInterruptionStatusIds.Resolved;

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
                RescheduledFromTestSessionCandidateId = currentAssignment.Id
            };

            await unitOfWork.Context.Set<TestSessionCandidate>().AddAsync(replacement, token);

            var applicantId = await unitOfWork.Context.Set<Invitation>()
                .AsNoTracking()
                .Where(x => x.Id == currentAssignment.InvitationId)
                .Select(x => x.ApplicantId)
                .SingleAsync(token);

            replacement.AddDomainEvent(new CandidateExamRescheduledDomainEvent(replacement.Id, applicantId,
                exam.TitleAr, exam.TitleEn, targetSlot.SlotDate, targetStart, room.Location!.NameAr,
                room.Location.NameEn, room.NameAr, room.NameEn, targetSession.SessionNo));

            await unitOfWork.GetEntityRepository<ActionLog>()
                .AddAsync(
                    new ActionLog
                    {
                        UserId = request.OfficerId,
                        LogType = ActionLogType.Employee,
                        ActionType = CandidateExamAttemptAudit.Rescheduled,
                        Section = CandidateExamAttemptAudit.Section,
                        EntityId = attempt.Id,
                        Notes = JsonSerializer.Serialize(new
                        {
                            testAttemptId = attempt.Id,
                            oldTestSessionCandidateId = currentAssignment.Id,
                            newTestSessionCandidateId = replacement.Id,
                            oldTestSessionId = currentSession.Id,
                            newTestSessionId = targetSession.Id,
                            interruptionId = interruption.Id,
                            reason,
                            rescheduledAt,
                            officerId = request.OfficerId
                        })
                    }, token);

            await unitOfWork.SaveChangesAsync(token);

            return Result.Ok(replacement.Id);
        }, cancellationToken);

    private static IResult<Guid> Unavailable() =>
        Result.Fail<Guid>(ErrorsCodes.CandidateExamRescheduleUnavailable);

    private static IResult<Guid> TargetInvalid() =>
        Result.Fail<Guid>(ErrorsCodes.CandidateExamRescheduleTargetSessionInvalid);
}
