using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Queries;

// No slot math here: the schedule's slots were generated and distributed once at creation
// (ScheduleAppointmentPlanner.GenerateSlots/DistributeCandidates) and persisted - the surplus as open
// "Held" rows. Reading those rows back is what keeps a reschedule inside the approved capacity.
public sealed class GetAvailableSlotsForRescheduleQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAvailableSlotsForRescheduleQuery, IResult<List<RescheduleSlotDto>>>
{
    public async Task<IResult<List<RescheduleSlotDto>>> Handle(GetAvailableSlotsForRescheduleQuery request, CancellationToken cancellationToken)
    {
        var appointments = unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet.AsNoTracking();

        var appointment = await appointments
            .Where(a => a.Id == request.AppointmentId && a.InterviewScheduleId == request.InterviewScheduleId)
            .Select(a => new
            {
                a.Id,
                a.InterviewCommitteeId,
                a.StartAt,
                a.EndAt,
                a.Status,
                a.InvitationId,
                ScheduleStatus = a.InterviewSchedule!.Status
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (appointment is null)
            return Result.Fail<List<RescheduleSlotDto>>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        if (appointment.ScheduleStatus is not (ScheduleStatus.Approved or ScheduleStatus.Returned))
            return Result.Fail<List<RescheduleSlotDto>>(new Error(ErrorsCodes.InterviewAppointmentRescheduleNotAllowed));

        // Same rule as InterviewAppointment.MarkRescheduled: only a candidate's live booking (Scheduled, or
        // Completed for the Final Review corrective reschedule the command checks further) can move.
        if (appointment.InvitationId is null || appointment.Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Completed))
            return Result.Fail<List<RescheduleSlotDto>>(new Error(ErrorsCodes.InterviewAppointmentNotScheduled));

        var now = DateTime.UtcNow;

        var slots = await appointments
            .Where(InterviewAppointment.IsOpenSlot)
            .Where(a => a.InterviewScheduleId == request.InterviewScheduleId
                && a.InterviewCommitteeId == appointment.InterviewCommitteeId
                && a.StartAt > now
                && a.Id != appointment.Id
                // A seat an earlier reschedule vacated at the candidate's own current time.
                && !(a.StartAt == appointment.StartAt && a.EndAt == appointment.EndAt))
            .OrderBy(a => a.StartAt)
            .Select(a => new RescheduleSlotDto(
                a.Id,
                a.StartAt.AsUtcOffset(),
                a.EndAt.AsUtcOffset(),
                a.RoomId,
                a.Room != null ? a.Room.NameAr : null,
                a.Room != null ? a.Room.NameEn : null,
                a.RemoteMeetingUrl))
            .ToListAsync(cancellationToken);

        return Result.Ok(slots);
    }
}
