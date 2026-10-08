namespace Application.Operation.Features.Employee.CandidateExamAttempts.DTOs;

public sealed record CandidateExamRescheduleOptionDto(
    Guid TestSessionId,
    string SessionNo,
    Guid TestSlotId,
    string SlotTitleAr,
    string? SlotTitleEn,
    DateOnly SlotDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    Guid RoomId,
    string RoomNameAr,
    string? RoomNameEn,
    int Capacity,
    int AssignedCount,
    int AvailableCapacity);
