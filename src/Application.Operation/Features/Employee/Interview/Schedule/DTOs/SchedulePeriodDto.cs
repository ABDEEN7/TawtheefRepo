namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// A period reconstructed from a persisted schedule's slots (see ScheduleAppointmentPlanner.ReconstructPeriods) -
// what the edit wizard needs to re-populate step 2. Periods are never stored themselves.
public sealed record SchedulePeriodDto(
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    Guid? RoomId,
    string? RoomNameAr,
    string? RoomNameEn,
    string? RemoteMeetingUrl,
    string? RemoteMeetingInstructions);
