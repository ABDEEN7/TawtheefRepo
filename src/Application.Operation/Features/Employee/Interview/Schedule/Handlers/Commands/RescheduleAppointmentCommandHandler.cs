using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
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

        // role : reschedule of an individual appointment is only allowed while the parent
        // schedule is Approved or Returned.
        if (appointment.InterviewSchedule!.Status is not (ScheduleStatus.Approved or ScheduleStatus.Returned))
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentRescheduleNotAllowed));

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

            var reportUnderReview = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
                .AnyAsync(r => r.InterviewScheduleId == appointment.InterviewScheduleId && r.Status == ResultReportStatus.UnderReview, cancellationToken);
            if (!reportUnderReview)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewResultReportNotReadyForReschedule));

            allowCompletedReschedule = true;
        }

        if (request.NewEndAt <= request.NewStartAt)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewSchedulePeriodInvalidTime));

        var hasLocation = appointment.InterviewType == InterviewType.InPerson
            ? request.RoomId is not null
            : !string.IsNullOrWhiteSpace(request.RemoteMeetingUrl);
        if (!hasLocation)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewSchedulePeriodMissingLocation));

        var newSlot = new GeneratedSlotDto(
            DateOnly.FromDateTime(request.NewStartAt), request.NewStartAt, request.NewEndAt,
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
            request.NewStartAt, request.NewEndAt, appointment.InvitationId, appointment.Id);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewAppointment),
            EntityId = appointment.Id,
            Action = InterviewAppointmentAuditActions.Rescheduled,
            OldValues = JsonSerializer.Serialize(new { appointment.StartAt, appointment.EndAt, appointment.RoomId, appointment.RemoteMeetingUrl }),
            NewValues = JsonSerializer.Serialize(new { NewAppointmentId = replacement.Id, replacement.StartAt, replacement.EndAt, replacement.RoomId, replacement.RemoteMeetingUrl }),
            Reason = request.Reason
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(replacement.Id);
    }
}
