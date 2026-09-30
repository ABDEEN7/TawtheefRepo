using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

// Moves the candidate into another open slot of the SAME schedule (see
// GetAvailableSlotsForRescheduleQuery) - never a free date/time, so the schedule's approved
// capacity/distribution is never bypassed. The old row is flipped to Rescheduled (keeping its
// RescheduleReason) and the target slot takes over the candidate, linked back via
// RescheduledFromAppointmentId. Only allowed while the parent InterviewSchedule is Approved or Returned.
public sealed record RescheduleAppointmentCommand(
    Guid AppointmentId,
    Guid TargetSlotId,
    string Reason) : IRequest<IResult<Guid>>;
