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
    DateTime? ApprovedAt,
    DateTime? ClosedAt,
    string? DecisionNotes,
    DateTime CreatedDate,
    IReadOnlyList<ExamResultReportCandidateDto> Candidates);

public sealed record ExamResultReportCandidateDto(
    Guid Id,
    Guid InvitationId,
    Guid? TestSessionCandidateId,
    Guid? TestAttemptId,

    string CandidateNameAr,
    string? CandidateNameEn,
    string? NationalNumber,

    int? SessionNo,
    string? SlotTitleAr,
    string? SlotTitleEn,
    DateOnly? SlotDate,
    TimeOnly? SlotStartTime,
    TimeOnly? SlotEndTime,

    ExamResultLookupDto? AttendanceStatus,
    ExamResultLookupDto? IdentityVerificationStatus,
    ExamResultLookupDto? CandidateExecutionStatus,
    ExamResultLookupDto? AttemptStatus,

    DateTime? AttemptStartedAt,
    DateTime? AttemptCompletedAt,

    decimal? SpecializedScore,
    bool? IsSpecializedQualified,
    decimal? EducationalScore,
    decimal? SkillsScore,
    decimal? FinalScore,
    int? Rank,

    ExamResultLookupDto ResultStatus,

    bool HasOpenInterruption,
    IReadOnlyList<ExamResultInterruptionDto> Interruptions,

    DateTime SnapshotAt);

public sealed record ExamResultInterruptionDto(
    Guid Id,
    DateTime InterruptedAt,
    string? Reason,
    ExamResultLookupDto Status,
    ExamResultLookupDto? ResolutionAction,
    string? ResolutionNotes,
    Guid? ResolvedById,
    DateTime? ResolvedAt);

public sealed record ExamResultLookupDto(
    Guid Id,
    string BackendName,
    string NameAr,
    string NameEn);
