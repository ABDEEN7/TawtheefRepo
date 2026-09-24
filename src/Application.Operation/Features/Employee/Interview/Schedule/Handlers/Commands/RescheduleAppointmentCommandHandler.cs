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
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Include(a => a.InterviewSchedule)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        // schedule.md: reschedule of an individual appointment is only allowed while the parent
        // schedule is Approved or Returned.
        if (appointment.InterviewSchedule!.Status is not (ScheduleStatus.Approved or ScheduleStatus.Returned))
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleNotAllowed));

        // The schedule's current (non-discarded) result report, if one was already generated. Held slots
        // have no candidate, so they're never part of a report and can move freely.
        var report = appointment.InvitationId is null
            ? null
            : await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
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

            if (report?.Status != ResultReportStatus.UnderReview)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewResultReportNotReadyForReschedule));

            allowCompletedReschedule = true;
        }

        var newStartAt = request.NewStartAt.UtcDateTime;
        var newEndAt = request.NewEndAt.UtcDateTime;

        if (newEndAt <= newStartAt)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewSchedulePeriodInvalidTime));

        var hasLocation = appointment.InterviewType == InterviewType.InPerson
            ? request.RoomId is not null
            : !string.IsNullOrWhiteSpace(request.RemoteMeetingUrl);
        if (!hasLocation)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewSchedulePeriodMissingLocation));

        var newSlot = new GeneratedSlotDto(
            newStartAt, newEndAt,
            request.RoomId, request.RemoteMeetingUrl, request.RemoteMeetingInstructions);

        var conflictResult = await ScheduleConflictChecker.ValidateNoConflictsAsync(
            unitOfWork, appointment.InterviewCommitteeId, [newSlot], cancellationToken, excludeAppointmentId: appointment.Id);
        if (conflictResult.IsFailed)
            return Result.Fail<Guid>(conflictResult.Errors);

        var markResult = appointment.MarkRescheduled(request.Reason, allowCompletedReschedule);
        if (markResult.IsFailed)
            return Result.Fail<Guid>(markResult.Errors);

        var replacement = appointment.InterviewSchedule.AddAppointment(
            appointment.InterviewCommitteeId, appointment.InterviewType, request.RoomId,
            request.RemoteMeetingUrl, request.RemoteMeetingInstructions,
            newStartAt, newEndAt, appointment.InvitationId, appointment.Id);

        // Add explicitly so EF generates the replacement's Id now - otherwise it's only assigned at
        // SaveChanges and the audit row below records NewAppointmentId as Guid.Empty.
        await unitOfWork.GetEntityRepository<InterviewAppointment>().AddAsync(replacement, cancellationToken);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewAppointment),
            EntityId = appointment.Id,
            Action = InterviewAppointmentAuditActions.Rescheduled,
            OldValues = JsonSerializer.Serialize(new { appointment.StartAt, appointment.EndAt, appointment.RoomId, appointment.RemoteMeetingUrl }),
            NewValues = JsonSerializer.Serialize(new { NewAppointmentId = replacement.Id, replacement.StartAt, replacement.EndAt, replacement.RoomId, replacement.RemoteMeetingUrl }),
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
                NewValues = JsonSerializer.Serialize(new { RescheduledAppointmentId = appointment.Id, NewAppointmentId = replacement.Id }),
                Reason = request.Reason
            }, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(replacement.Id);
    }
}
