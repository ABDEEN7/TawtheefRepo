using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

// Invites every live candidate of the schedule who hasn't been invited yet; already-invited ones are
// skipped (reminders stay a per-row action), so pressing it twice sends nothing new. Returns how many
// invitations were queued.
public sealed record SendScheduleInvitationsCommand(Guid ScheduleId) : IRequest<IResult<int>>;
