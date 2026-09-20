using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.TestSlots.DTOs;

public sealed class TestSlotDetailsDto
{
    public Guid Id { get; init; }
    public required string TitleAr { get; init; }
    public string? TitleEn { get; init; }
    public required string RoomName { get; init; }
    public int RoomCapacity { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public required DropdownOptions Status { get; init; }
    public int CandidateCount { get; init; }
    public int TotalCandidates { get; init; }
    public int PresentCandidates { get; init; }
    public int WaitingCandidates { get; init; }
    public int AbsentCandidates { get; init; }
    public required List<TestSlotStaffDetailsDto> Staff { get; init; }
    public bool IsCurrentUserRoomHead { get; init; }
    public bool HasRevealableAccessCode { get; init; }
}

public sealed record TestSlotStaffDetailsDto(Guid UserId, string Name, string RoleName);
public sealed record TestSlotCandidateListItemDto(
    Guid CandidateId,
    string CandidateName,
    string? Qid,
    string ExamTitle,
    string? JobTitle,
    int SessionNo,
    string ExamNo,
    string AttendanceStatus,
    string AttemptStatus,
    string? NotificationStatus);
public sealed record TestSlotSessionDto(Guid Id, int SessionNo, string ExamNo, string StatusName, int CandidateCount);
public sealed record TestSlotAccessCodeDto(string AccessCode);
