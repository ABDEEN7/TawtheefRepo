namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// One concrete interview time slot computed server-side from a PeriodInputDto - never trust a
// client-submitted slot list as the source of truth (same as frontend demo).
public sealed record GeneratedSlotDto(
    DateOnly Date,
    DateTime StartAt,
    DateTime EndAt,
    Guid? RoomId,
    string? RemoteMeetingUrl,
    string? RemoteMeetingInstructions);
