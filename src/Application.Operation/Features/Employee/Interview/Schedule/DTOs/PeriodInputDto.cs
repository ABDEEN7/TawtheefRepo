namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// One committee-availability window from the wizard's period table. Never persisted as its own
// row - it only exists to be exploded into concrete InterviewAppointment slots at submit time.
public sealed record PeriodInputDto(
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    Guid? RoomId,
    string? RemoteMeetingUrl,
    string? RemoteMeetingInstructions);
