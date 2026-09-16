using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

// Never mutates the old row's time in place - it flips the old appointment to Rescheduled (keeping
// its RescheduleReason) and inserts a brand-new Scheduled row linked back via
// RescheduledFromAppointmentId, re-running the exact same conflict checks as initial scheduling
// Only allowed while the parent InterviewSchedule is Approved or Returned.
public sealed record RescheduleAppointmentCommand(
    Guid AppointmentId,
    DateTime NewStartAt,
    DateTime NewEndAt,
    Guid? RoomId,
    string? RemoteMeetingUrl,
    string? RemoteMeetingInstructions,
    string Reason) : IRequest<IResult<Guid>>;
