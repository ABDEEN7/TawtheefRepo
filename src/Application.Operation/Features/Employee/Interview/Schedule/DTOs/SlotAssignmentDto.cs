namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// A manual override pairing one generated slot (identified by its computed start time) with a
// candidate. Slots left out of this list are auto-filled from the remaining eligible pool.
public sealed record SlotAssignmentDto(DateTime SlotStartAt, Guid InvitationId);
