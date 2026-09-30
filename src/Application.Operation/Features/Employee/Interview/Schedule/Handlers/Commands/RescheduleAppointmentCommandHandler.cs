using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class RescheduleAppointmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RescheduleAppointmentCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointments = unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet;

        var appointment = await appointments
            .Include(a => a.InterviewSchedule)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        // schedule.md: reschedule of an individual appointment is only allowed while the parent
        // schedule is Approved or Returned.
        if (appointment.InterviewSchedule!.Status is not (ScheduleStatus.Approved or ScheduleStatus.Returned))
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleNotAllowed));

        // Only a real candidate can be moved - an open slot has nobody to reschedule.
        if (appointment.InvitationId is not { } invitationId)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentNotScheduled));

        if (request.TargetSlotId == appointment.Id)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleSameSlot));

        // The target must be an open slot of this same schedule/committee. Tracked, since the domain
        // method below assigns the candidate to it.
        var targetSlot = await appointments
            .FirstOrDefaultAsync(a => a.Id == request.TargetSlotId
                && a.InterviewScheduleId == appointment.InterviewScheduleId
                && a.InterviewCommitteeId == appointment.InterviewCommitteeId, cancellationToken);
        if (targetSlot is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleSlotNotFound));

        if (!InterviewAppointment.IsOpenSlotCompiled(targetSlot))
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleSlotTaken));

        if (targetSlot.StartAt <= DateTime.UtcNow)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleSlotInPast));

        // Same wall time as the candidate's current appointment (e.g. a seat an earlier reschedule
        // vacated) - moving there changes nothing.
        if (targetSlot.StartAt == appointment.StartAt && targetSlot.EndAt == appointment.EndAt)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleSameSlot));

        // The schedule's current (non-discarded) result report, if one was already generated.
        var report = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
            .Include(r => r.Candidates)
            .FirstOrDefaultAsync(r => r.InterviewScheduleId == appointment.InterviewScheduleId, cancellationToken);

        // Final decisions were already pushed to the candidates' invitations - the candidate set is frozen.
        if (report is { IsFinalized: true })
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewResultReportFinalizedNoReschedule));

        // Corrective reschedule during Final Review: a Completed appointment is normally locked
        // (EnsureEditable), but a candidate whose interview had a problem may need one redo once the
        // schedule's result report exists and is still under review. Narrowly scoped - only allowed
        // once (a row that is itself already a replacement can't be corrective-rescheduled again "for
        // now"), and only via this explicit flag, never a blanket loosening of the Completed guard.
        var allowCompletedReschedule = false;
        if (appointment.Status == AppointmentStatus.Completed)
        {
            if (appointment.RescheduledFromAppointmentId is not null)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentAlreadyRescheduledOnce));

            if (report is null || !report.IsCommitteeReviewEditable)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewResultReportNotReadyForReschedule));

            allowCompletedReschedule = true;
        }

        // The open slot already occupies its time/room in the global conflict index, so it only has
        // to be re-checked against everything except itself and the row being vacated.
        var targetSlotDto = new GeneratedSlotDto(
            targetSlot.StartAt, targetSlot.EndAt,
            targetSlot.RoomId, targetSlot.RemoteMeetingUrl, targetSlot.RemoteMeetingInstructions);
        var conflictResult = await ScheduleConflictChecker.ValidateNoConflictsAsync(
            unitOfWork, appointment.InterviewCommitteeId, [targetSlotDto], cancellationToken,
            excludeAppointmentIds: [appointment.Id, targetSlot.Id]);
        if (conflictResult.IsFailed)
            return Result.Fail<Guid>(conflictResult.Errors);

        var originalStatus = appointment.Status;

        var markResult = appointment.MarkRescheduled(request.Reason, allowCompletedReschedule);
        if (markResult.IsFailed)
            return Result.Fail<Guid>(markResult.Errors);

        var acceptResult = targetSlot.AcceptRescheduledCandidate(invitationId, appointment.Id);
        if (acceptResult.IsFailed)
            return Result.Fail<Guid>(acceptResult.Errors);

        // The candidate's old seat goes back into the pool as an open slot, so the schedule's capacity
        // is preserved for the next reschedule. A seat already in the past (e.g. the corrective
        // reschedule of a Completed interview) can't be booked anyway.
        InterviewAppointment? reopenedSlot = null;
        if (appointment.StartAt > DateTime.UtcNow)
        {
            reopenedSlot = appointment.InterviewSchedule.AddAppointment(
                appointment.InterviewCommitteeId, appointment.InterviewType, appointment.RoomId,
                appointment.RemoteMeetingUrl, appointment.RemoteMeetingInstructions,
                appointment.StartAt, appointment.EndAt, invitationId: null);
            // Add explicitly so EF generates the Id now - the audit row below records it.
            await unitOfWork.GetEntityRepository<InterviewAppointment>().AddAsync(reopenedSlot, cancellationToken);
        }

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewAppointment),
            EntityId = appointment.Id,
            Action = InterviewAppointmentAuditActions.Rescheduled,
            OldValues = JsonSerializer.Serialize(new { appointment.StartAt, appointment.EndAt, appointment.RoomId, appointment.RemoteMeetingUrl }),
            NewValues = JsonSerializer.Serialize(new
            {
                NewAppointmentId = targetSlot.Id,
                targetSlot.StartAt,
                targetSlot.EndAt,
                targetSlot.RoomId,
                targetSlot.RemoteMeetingUrl,
                ReopenedSlotId = reopenedSlot?.Id
            }),
            Reason = request.Reason
        }, cancellationToken);

        // The report no longer matches the schedule's candidates (this one now has a new, not-yet-held
        // appointment), so it is discarded - soft deleted, still listed as "Discarded" on the approval
        // page - and InterviewResultCalculationService regenerates a fresh one once the replacement
        // appointment is finished like any other.
        if (report is not null)
        {
            // Candidates go with it: UQ_ResultCandidate (one live result per appointment) would otherwise
            // block the regenerated report from re-scoring the unchanged appointments.
            await unitOfWork.GetEntityRepository<InterviewResultCandidate>().DeleteRangeAsync(report.Candidates);
            await unitOfWork.GetEntityRepository<InterviewResultReport>().DeleteAsync(report);
            await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
            {
                EntityType = nameof(InterviewResultReport),
                EntityId = report.Id,
                Action = InterviewResultReportAuditActions.Discarded,
                OldValues = JsonSerializer.Serialize(new { report.Status }),
                NewValues = JsonSerializer.Serialize(new { RescheduledAppointmentId = appointment.Id, NewAppointmentId = targetSlot.Id }),
                Reason = request.Reason
            }, cancellationToken);
        }

        // Everything above was read without locks, so two users can pass the same checks at once:
        // both grabbing the last seat of a slot, or both moving the same candidate. Each row is
        // therefore claimed with a conditional UPDATE (compare-and-swap on the status validated above)
        // inside the transaction - SQL Server serializes the two UPDATEs on the row lock, the loser
        // re-reads the committed row, matches 0 rows and the whole transaction rolls back. The lambda
        // returns a non-generic Result so ExecuteInTransactionAsync rolls back on failure, and it only
        // holds the claims + SaveChanges so the execution strategy can safely retry it.
        var transactionResult = await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var vacatedClaimed = await appointments
                .Where(a => a.Id == appointment.Id && a.Status == originalStatus)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Rescheduled), ct);
            if (vacatedClaimed == 0)
                return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentConcurrentUpdate));

            var slotClaimed = await appointments
                .Where(a => a.Id == targetSlot.Id && a.Status == AppointmentStatus.Held && a.InvitationId == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Status, AppointmentStatus.Scheduled)
                    .SetProperty(a => a.InvitationId, invitationId), ct);
            if (slotClaimed == 0)
                return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentRescheduleSlotTaken));

            await unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }, cancellationToken);

        return transactionResult.IsFailed
            ? Result.Fail<Guid>(transactionResult.Errors)
            : Result.Ok(targetSlot.Id);
    }
}
