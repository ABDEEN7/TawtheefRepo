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
    DateTimeOffset SnapshotAt,
    bool HasAttempt,
    ExamResultLookupDto? AttemptStatus,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyList<ExamResultAttemptPartDto> Parts);
