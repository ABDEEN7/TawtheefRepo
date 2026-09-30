using Application.Operation.Features.Employee.Interview.Dashboard.DTOs;
using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using Application.Operation.Features.Employee.Interview.Dashboard.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Handlers.Queries;

// Every number is a server-side GROUP BY / COUNT over the scoped queries - nothing is loaded row by row.
// State meanings come straight from the domain enums; the only groupings made here are:
//  - "Scheduled" schedules = Approved + ReadyForExecution (approved, execution not started/closed yet).
//  - "Interviews completed" = appointment reached UnderEvaluation/Completed/Closed (interview held).
//  - "Evaluations completed" = Completed/Closed (evaluation quorum reached, see SubmitMemberEvaluation).
public sealed class GetInterviewDashboardOverviewQueryHandler(
    InterviewDashboardAccessResolver accessResolver,
    InterviewDashboardQueryScope scope)
    : IRequestHandler<GetInterviewDashboardOverviewQuery, IResult<InterviewDashboardOverviewDto>>
{
    private const int MaxDailyPoints = 62;

    public async Task<IResult<InterviewDashboardOverviewDto>> Handle(
        GetInterviewDashboardOverviewQuery request, CancellationToken cancellationToken)
    {
        var access = await accessResolver.ResolveAsync(cancellationToken);

        var templates = access.Templates ? await ReadTemplatesAsync(request, cancellationToken) : null;
        var committees = access.Committees ? await ReadCommitteesAsync(request, cancellationToken) : null;
        var schedules = access.Execution ? await ReadSchedulesAsync(request, access, cancellationToken) : null;
        var candidates = access.Execution ? await ReadCandidatesAsync(request, access, cancellationToken) : null;
        var issues = access.Execution ? await ReadIssuesAsync(request, access, cancellationToken) : null;
        var activity = access.Execution ? await ReadActivityAsync(request, access, cancellationToken) : null;
        var results = access.Results ? await ReadResultsAsync(request, cancellationToken) : null;

        return Result.Ok(new InterviewDashboardOverviewDto(
            new InterviewDashboardSectionsDto(access.Templates, access.Committees, access.Execution, access.Results),
            templates, committees, schedules, candidates, issues, results, activity));
    }

    private async Task<InterviewDashboardTemplateKpisDto> ReadTemplatesAsync(
        InterviewDashboardFilter filter, CancellationToken cancellationToken)
    {
        var templates = scope.Templates(filter);
        var approvedVersions = scope.TemplateVersions().Where(v => v.Status == TemplateVersionStatus.Approved);

        var active = await templates.CountAsync(cancellationToken);
        var approved = await templates
            .CountAsync(t => approvedVersions.Any(v => v.InterviewTemplateId == t.Id), cancellationToken);

        return new InterviewDashboardTemplateKpisDto(active, approved);
    }

    private async Task<InterviewDashboardCommitteeKpisDto> ReadCommitteesAsync(
        InterviewDashboardFilter filter, CancellationToken cancellationToken)
    {
        var byStatus = await scope.Committees(filter)
            .GroupBy(c => c.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return new InterviewDashboardCommitteeKpisDto(
            byStatus.Sum(x => x.Count),
            byStatus.Where(x => x.Key == CommitteeStatus.Approved).Sum(x => x.Count),
            byStatus.Select(x => new InterviewDashboardCountDto((int)x.Key, x.Count)).ToList());
    }

    private async Task<InterviewDashboardScheduleKpisDto> ReadSchedulesAsync(
        InterviewDashboardFilter filter, InterviewDashboardAccess access, CancellationToken cancellationToken)
    {
        var byStatus = await scope.Schedules(filter, access)
            .GroupBy(s => s.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int Count(params ScheduleStatus[] statuses) => byStatus.Where(x => statuses.Contains(x.Key)).Sum(x => x.Count);

        return new InterviewDashboardScheduleKpisDto(
            byStatus.Sum(x => x.Count),
            Count(ScheduleStatus.Approved, ScheduleStatus.ReadyForExecution),
            Count(ScheduleStatus.InProgress),
            Count(ScheduleStatus.Closed),
            byStatus.Select(x => new InterviewDashboardCountDto((int)x.Key, x.Count)).ToList());
    }

    private async Task<InterviewDashboardCandidateKpisDto> ReadCandidatesAsync(
        InterviewDashboardFilter filter, InterviewDashboardAccess access, CancellationToken cancellationToken)
    {
        var live = scope.LiveAppointments(filter, access);
        var lateIssues = scope.LateIssues();

        var byStatusAndAttendance = await live
            .GroupBy(a => new
            {
                a.Status,
                a.AttendanceStatus,
                HasLateIssue = lateIssues.Any(i => i.InterviewAppointmentId == a.Id)
            })
            .Select(g => new { g.Key.Status, g.Key.AttendanceStatus, g.Key.HasLateIssue, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var byType = await live
            .GroupBy(a => a.InterviewType)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Superseded/withdrawn rows are not live, but still tell the operational story.
        var removed = await scope.Appointments(filter, access)
            .Where(a => a.InvitationId != null
                && (a.Status == AppointmentStatus.Rescheduled || a.Status == AppointmentStatus.Cancelled))
            .GroupBy(a => a.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Exclusive buckets for the attendance pie (see InterviewDashboardQueryScope.LateIssues):
        // Present / not recorded exclude late candidates, which are counted under Late instead.
        int Attendance(AttendanceStatus? status) =>
            byStatusAndAttendance.Where(x => x.AttendanceStatus == status).Sum(x => x.Count);
        int OnTime(AttendanceStatus? status) =>
            byStatusAndAttendance.Where(x => x.AttendanceStatus == status && !x.HasLateIssue).Sum(x => x.Count);
        int Status(params AppointmentStatus[] statuses) =>
            byStatusAndAttendance.Where(x => statuses.Contains(x.Status)).Sum(x => x.Count);
        var late = byStatusAndAttendance
            .Where(x => x.AttendanceStatus == AttendanceStatus.Late
                || (x.HasLateIssue && (x.AttendanceStatus == AttendanceStatus.Present || x.AttendanceStatus == null)))
            .Sum(x => x.Count);

        return new InterviewDashboardCandidateKpisDto(
            byStatusAndAttendance.Sum(x => x.Count),
            OnTime(AttendanceStatus.Present),
            late,
            Attendance(AttendanceStatus.NoShow),
            Attendance(AttendanceStatus.Withdrew),
            OnTime(null),
            Status(AppointmentStatus.UnderEvaluation, AppointmentStatus.Completed, AppointmentStatus.Closed),
            Status(AppointmentStatus.Completed, AppointmentStatus.Closed),
            removed.Where(x => x.Key == AppointmentStatus.Rescheduled).Sum(x => x.Count),
            removed.Where(x => x.Key == AppointmentStatus.Cancelled).Sum(x => x.Count),
            byStatusAndAttendance
                .GroupBy(x => x.Status)
                .Select(g => new InterviewDashboardCountDto((int)g.Key, g.Sum(x => x.Count)))
                .ToList(),
            byType.Select(x => new InterviewDashboardCountDto((int)x.Key, x.Count)).ToList());
    }

    private async Task<InterviewDashboardIssueKpisDto> ReadIssuesAsync(
        InterviewDashboardFilter filter, InterviewDashboardAccess access, CancellationToken cancellationToken)
    {
        var rows = await scope.Issues(filter, access)
            .GroupBy(i => new { i.IssueType, i.Status, i.IsBlocking })
            .Select(g => new { g.Key.IssueType, g.Key.Status, g.Key.IsBlocking, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return new InterviewDashboardIssueKpisDto(
            rows.Sum(x => x.Count),
            rows.Where(x => x.Status == OperationalIssueStatus.Open).Sum(x => x.Count),
            rows.Where(x => x.Status == OperationalIssueStatus.Open && x.IsBlocking).Sum(x => x.Count),
            rows.Where(x => x.Status == OperationalIssueStatus.Resolved).Sum(x => x.Count),
            rows.Where(x => x.Status == OperationalIssueStatus.Waived).Sum(x => x.Count),
            rows.GroupBy(x => x.IssueType)
                .Select(g => new InterviewDashboardIssueTypeCountDto(
                    (int)g.Key,
                    g.Where(x => x.Status == OperationalIssueStatus.Open).Sum(x => x.Count),
                    g.Where(x => x.Status != OperationalIssueStatus.Open).Sum(x => x.Count)))
                .OrderBy(x => x.Type)
                .ToList());
    }

    private async Task<InterviewDashboardResultKpisDto> ReadResultsAsync(
        InterviewDashboardFilter filter, CancellationToken cancellationToken)
    {
        var candidates = scope.ResultCandidates(filter);

        var byDecision = await candidates
            .GroupBy(c => new { c.FinalDecision, c.IsQualified })
            .Select(g => new { g.Key.FinalDecision, g.Key.IsQualified, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var reportsByStatus = await candidates
            .Select(c => new { c.InterviewResultReportId, c.InterviewResultReport!.Status })
            .Distinct()
            .GroupBy(x => x.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int Decision(FinalDecision? decision) => byDecision.Where(x => x.FinalDecision == decision).Sum(x => x.Count);

        return new InterviewDashboardResultKpisDto(
            byDecision.Sum(x => x.Count),
            byDecision.Where(x => x.IsQualified).Sum(x => x.Count),
            byDecision.Where(x => !x.IsQualified).Sum(x => x.Count),
            Decision(FinalDecision.CandidateForHiringProcess),
            Decision(FinalDecision.WaitingList),
            Decision(FinalDecision.Rejected),
            Decision(FinalDecision.NeedsAction),
            Decision(FinalDecision.NoShow),
            Decision(null),
            reportsByStatus.Select(x => new InterviewDashboardCountDto((int)x.Key, x.Count)).ToList());
    }

    // Live appointments per day in the viewer's timezone (StartAt is UTC, shifted by UtcOffsetMinutes). Day buckets for spans up to 62 days (gaps filled with zeros so the
    // line is continuous), month buckets beyond that.
    private async Task<InterviewDashboardActivityDto> ReadActivityAsync(
        InterviewDashboardFilter filter, InterviewDashboardAccess access, CancellationToken cancellationToken)
    {
        var offset = filter.UtcOffsetMinutes;
        var days = await scope.LiveAppointments(filter, access)
            .GroupBy(a => a.StartAt.AddMinutes(offset).Date)
            .Select(g => new
            {
                Day = g.Key,
                Scheduled = g.Count(),
                EvaluationsCompleted = g.Count(a => a.Status == AppointmentStatus.Completed || a.Status == AppointmentStatus.Closed),
                NoShow = g.Count(a => a.AttendanceStatus == AttendanceStatus.NoShow)
            })
            .ToListAsync(cancellationToken);

        var points = days
            .Select(d => new InterviewDashboardActivityPointDto(
                DateOnly.FromDateTime(d.Day), d.Scheduled, d.EvaluationsCompleted, d.NoShow))
            .ToList();

        var first = filter.FromDate ?? points.Select(p => (DateOnly?)p.Period).Min();
        var last = filter.ToDate ?? points.Select(p => (DateOnly?)p.Period).Max();
        if (first is null || last is null || last < first)
            return new InterviewDashboardActivityDto("Day", points.OrderBy(p => p.Period).ToList());

        var span = last.Value.DayNumber - first.Value.DayNumber + 1;
        if (span <= MaxDailyPoints)
        {
            var byDay = points.ToDictionary(p => p.Period);
            var filled = Enumerable.Range(0, span)
                .Select(offset => first.Value.AddDays(offset))
                .Select(day => byDay.GetValueOrDefault(day) ?? new InterviewDashboardActivityPointDto(day, 0, 0, 0))
                .ToList();
            return new InterviewDashboardActivityDto("Day", filled);
        }

        var monthly = points
            .GroupBy(p => new DateOnly(p.Period.Year, p.Period.Month, 1))
            .Select(g => new InterviewDashboardActivityPointDto(
                g.Key, g.Sum(p => p.Scheduled), g.Sum(p => p.EvaluationsCompleted), g.Sum(p => p.NoShow)))
            .OrderBy(p => p.Period)
            .ToList();
        return new InterviewDashboardActivityDto("Month", monthly);
    }
}
