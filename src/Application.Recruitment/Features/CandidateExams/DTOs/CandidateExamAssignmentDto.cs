using Tawtheef.Application.Common.Models;

namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CandidateExamAssignmentDto
{
    public Guid TestSessionCandidateId { get; init; }
    public Guid TestSessionId { get; init; }
    public Guid ExamId { get; init; }
    public Guid? TestAttemptId { get; init; }
    public required string ExamNo { get; init; }
    public required string ExamTitleAr { get; init; }
    public string? ExamTitleEn { get; init; }
    public required string JobTitleAr { get; init; }
    public required string JobTitleEn { get; init; }
    public int TotalQuestions { get; init; }
    public bool AllowPreviousQuestion { get; init; }
    public int SessionNo { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public required string RoomNameAr { get; init; }
    public string? RoomNameEn { get; init; }
    public required string LocationNameAr { get; init; }
    public string? LocationNameEn { get; init; }
    public required DropdownOptions ExamStatus { get; init; }
    public required DropdownOptions SlotStatus { get; init; }
    public required DropdownOptions SessionStatus { get; init; }
    public required DropdownOptions CandidateStatus { get; init; }
    public required DropdownOptions AttendanceStatus { get; init; }
    public required DropdownOptions IdentityVerificationStatus { get; init; }
    public DropdownOptions? AttemptStatus { get; init; }
    public DateTime? AuthorizedAt { get; init; }
    public required string EntryState { get; init; }
}
