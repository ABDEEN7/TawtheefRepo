namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamResultCandidateAttemptDetailsDto(
    Guid ExamResultCandidateId,
    Guid InvitationId,
    Guid? TestSessionCandidateId,
    Guid? TestAttemptId,
    ExamResultLookupDto ResultStatus,
    decimal? SpecializedScore,
    decimal? EducationalScore,
    decimal? SkillsScore,
    decimal? FinalScore,
    int? Rank,
    DateTime SnapshotAt,
    bool HasAttempt,
    ExamResultLookupDto? AttemptStatus,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    IReadOnlyList<ExamResultAttemptPartDto> Parts);
