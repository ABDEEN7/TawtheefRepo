namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamResultReportDetailsDto(
    Guid Id,
    Guid ExamId,
    string ExamNo,
    string ExamTitleAr,
    string? ExamTitleEn,
    Guid JobId,
    string JobNameAr,
    string? JobNameEn,
    decimal SpecializedQualificationThreshold,
    ExamResultLookupDto Status,
    Guid? ApprovedById,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? ClosedAt,
    string? DecisionNotes,
    DateTimeOffset CreatedDate,
    IReadOnlyList<ExamResultReportCandidateDto> Candidates);

public sealed record ExamResultReportCandidateDto(
    Guid Id,
    Guid InvitationId,
    Guid? TestSessionCandidateId,
    Guid? TestAttemptId,
    string CandidateNameAr,
    string? CandidateNameEn,
    string? NationalNumber,
    string? SessionNo,
    string? SlotTitleAr,
    string? SlotTitleEn,
    DateOnly? SlotDate,
    TimeOnly? SlotStartTime,
    TimeOnly? SlotEndTime,
    ExamResultLookupDto? AttendanceStatus,
    ExamResultLookupDto? IdentityVerificationStatus,
    ExamResultLookupDto? CandidateExecutionStatus,
    ExamResultLookupDto? AttemptStatus,
    DateTimeOffset? AttemptStartedAt,
    DateTimeOffset? AttemptCompletedAt,
    decimal? SpecializedScore,
    bool? IsSpecializedQualified,
    decimal? EducationalScore,
    decimal? SkillsScore,
    decimal? FinalScore,
    int? Rank,
    ExamResultLookupDto ResultStatus,
    bool HasOpenInterruption,
    IReadOnlyList<ExamResultInterruptionDto> Interruptions,
    DateTimeOffset SnapshotAt);

public sealed record ExamResultInterruptionDto(
    Guid Id,
    DateTimeOffset InterruptedAt,
    string? Reason,
    ExamResultLookupDto Status,
    ExamResultLookupDto? ResolutionAction,
    string? ResolutionNotes,
    Guid? ResolvedById,
    DateTimeOffset? ResolvedAt);

public sealed record ExamResultLookupDto(Guid Id, string BackendName, string NameAr, string NameEn);
