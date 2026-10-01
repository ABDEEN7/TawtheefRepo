using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionEditDto(
    Guid TestSessionId,
    string SessionNo,
    Guid ExamId,
    Guid StatusId,
    TestSessionGenderFilter? GenderFilter,
    TestSessionNationalityFilter? NationalityFilter,
    IReadOnlyList<Guid> InvitationIds,
    Guid? TestSlotId,
    string? SlotName,
    DateOnly? SlotDate,
    TimeOnly? SlotStartTime,
    TimeOnly? SlotEndTime,
    Guid? RoomId,
    string? RoomName,
    int? RoomCapacity,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    int AvailableCapacity,
    string? DecisionNote,
    string? DecisionByName,
    DateTime? DecisionAt,
    string? StatusBackendName);
