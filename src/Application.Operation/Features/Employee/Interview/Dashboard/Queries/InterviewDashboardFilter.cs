using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Queries;

// Shared filter set for every Interview Dashboard query. FromDate/ToDate are calendar dates in the viewer's
// timezone (ToDate inclusive); appointment StartAt is a UTC instant, so UtcOffsetMinutes (the viewer's offset
// from UTC, e.g. 180 for Qatar) shifts the day boundaries and the activity chart's day buckets.
// Every filter narrows the same appointment universe, so KPIs, charts and detail tables always agree.
public abstract record InterviewDashboardFilter
{
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int UtcOffsetMinutes { get; init; }
    public Guid? JobId { get; init; }
    public Guid? CommitteeId { get; init; }
    public Guid? ScheduleId { get; init; }
    public InterviewType? InterviewType { get; init; }
    public ScheduleStatus? ScheduleStatus { get; init; }
    public ResultReportStatus? ResultReportStatus { get; init; }
    public FinalDecision? FinalDecision { get; init; }

    // True when a filter only an appointment can answer is set - schedules then count only if they have
    // at least one matching appointment.
    internal bool HasAppointmentLevelFilter =>
        FromDate.HasValue || ToDate.HasValue || CommitteeId.HasValue || InterviewType.HasValue || FinalDecision.HasValue;
}

public abstract record InterviewDashboardPagedFilter : InterviewDashboardFilter
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
