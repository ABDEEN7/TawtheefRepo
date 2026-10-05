using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.DTOs;

// ReportCode/ReportStatus are only filled for callers who may see result reports.
public sealed record InterviewDashboardScheduleRowDto(
    Guid ScheduleId,
    string TitleAr,
    string? TitleEn,
    string? JobNameAr,
    string? JobNameEn,
    string? CommitteeNameAr,
    string? CommitteeNameEn,
    ScheduleStatus Status,
    DateTimeOffset? FirstAppointmentAt,
    DateTimeOffset? LastAppointmentAt,
    int Candidates,
    int Present,
    int NoShow,
    int EvaluationsCompleted,
    int OpenIssues,
    string? ReportCode,
    ResultReportStatus? ReportStatus);

// StartAt is a UTC instant ("+00:00"), same as AppointmentDto.StartAt.
// Evaluation progress is a count only - no member identities or scores.
public sealed record InterviewDashboardCandidateRowDto(
    Guid AppointmentId,
    Guid ScheduleId,
    string? CandidateNameAr,
    string? CandidateNameEn,
    string? JobNameAr,
    string? JobNameEn,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    DateTimeOffset StartAt,
    InterviewType InterviewType,
    AppointmentStatus Status,
    AttendanceStatus? AttendanceStatus,
    // Arrived late (InterviewAppointment.IsLateCandidate): attendance stays Present, the UI adds a hint.
    bool IsLate,
    int SubmittedEvaluations,
    int RequiredEvaluations);

public sealed record InterviewDashboardResultRowDto(
    Guid CandidateId,
    Guid ScheduleId,
    string ReportCode,
    ResultReportStatus ReportStatus,
    string? CandidateNameAr,
    string? CandidateNameEn,
    string? JobNameAr,
    string? JobNameEn,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    decimal FinalScore,
    decimal? QualificationScore,
    bool IsQualified,
    FinalDecision? FinalDecision,
    DateTimeOffset? DecidedAt);

public sealed record InterviewDashboardIssueRowDto(
    Guid IssueId,
    Guid ScheduleId,
    Guid AppointmentId,
    OperationalIssueType IssueType,
    OperationalIssueStatus Status,
    bool IsBlocking,
    string? Description,
    string? CandidateNameAr,
    string? CandidateNameEn,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    DateTimeOffset CreatedDate,
    DateTimeOffset? ResolvedAt);

public sealed record InterviewDashboardLookupDto(Guid Id, string NameAr, string? NameEn, string? HintAr, string? HintEn);
