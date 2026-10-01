namespace Application.Operation.Features.Employee.Exams.ResultReport.DTOs;

public sealed record ExamResultCandidateRescheduleOptionDto(
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
