using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Queries;

// Feeds the reschedule dialog's slot selector: the open, future slots of the appointment's own
// schedule. RescheduleAppointmentCommand re-validates the chosen slot server-side.
public sealed record GetAvailableSlotsForRescheduleQuery(Guid InterviewScheduleId, Guid AppointmentId)
    : IRequest<IResult<List<RescheduleSlotDto>>>;
