namespace Application.Operation.Features.Employee.Interview.Dashboard.DTOs;

// Each section is null when the caller lacks the permission that section's source module requires -
// the client hides it rather than showing zeros the user isn't entitled to.
public sealed record InterviewDashboardOverviewDto(
    InterviewDashboardSectionsDto Sections,
    InterviewDashboardTemplateKpisDto? Templates,
    InterviewDashboardCommitteeKpisDto? Committees,
    InterviewDashboardScheduleKpisDto? Schedules,
    InterviewDashboardCandidateKpisDto? Candidates,
    InterviewDashboardIssueKpisDto? Issues,
    InterviewDashboardResultKpisDto? Results,
    InterviewDashboardActivityDto? Activity);

public sealed record InterviewDashboardSectionsDto(bool Templates, bool Committees, bool Execution, bool Results);

// Key = the domain enum's numeric value; Count = rows in that state.
public sealed record InterviewDashboardCountDto(int Key, int Count);

public sealed record InterviewDashboardTemplateKpisDto(int Active, int Approved);

public sealed record InterviewDashboardCommitteeKpisDto(int Total, int Approved, List<InterviewDashboardCountDto> ByStatus);

public sealed record InterviewDashboardScheduleKpisDto(
    int Total,
    int Scheduled,
    int InProgress,
    int Closed,
    List<InterviewDashboardCountDto> ByStatus);

public sealed record InterviewDashboardCandidateKpisDto(
    int Scheduled,
    int Present,
    int Late,
    int NoShow,
    int Withdrew,
    int AttendanceNotRecorded,
    int InterviewsCompleted,
    int EvaluationsCompleted,
    int Rescheduled,
    int Cancelled,
    List<InterviewDashboardCountDto> ByAppointmentStatus,
    List<InterviewDashboardCountDto> ByInterviewType);

public sealed record InterviewDashboardIssueTypeCountDto(int Type, int Open, int Closed);

public sealed record InterviewDashboardIssueKpisDto(
    int Total,
    int Open,
    int OpenBlocking,
    int Resolved,
    int Waived,
    List<InterviewDashboardIssueTypeCountDto> ByType);

public sealed record InterviewDashboardResultKpisDto(
    int Candidates,
    int Qualified,
    int NotQualified,
    int CandidateForHiringProcess,
    int WaitingList,
    int Rejected,
    int NeedsAction,
    int NoShow,
    int PendingDecision,
    List<InterviewDashboardCountDto> ReportsByStatus);

// Granularity: "Day" when the covered span is <= 62 days, otherwise "Month" (Period = first of month).
public sealed record InterviewDashboardActivityDto(string Granularity, List<InterviewDashboardActivityPointDto> Points);

public sealed record InterviewDashboardActivityPointDto(DateOnly Period, int Scheduled, int EvaluationsCompleted, int NoShow);
