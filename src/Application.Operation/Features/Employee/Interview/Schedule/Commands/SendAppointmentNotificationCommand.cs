using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

// Gated on the parent InterviewSchedule being Approved or Closed. First call sets
// InvitationSentAt; every later call is a reminder (LastReminderSentAt + ReminderCount).
public sealed record SendAppointmentNotificationCommand(Guid AppointmentId) : IRequest<IResult<Unit>>;
