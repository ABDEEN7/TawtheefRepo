using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Services;

// Builds the filtered, permission-scoped IQueryables every dashboard query aggregates over, so the
// overview and the detail tables can never disagree about which rows are "in". Nothing is materialised
// here - callers compose GROUP BY / COUNT / paging on top and let SQL Server do the work.
public sealed class InterviewDashboardQueryScope(IUnitOfWork unitOfWork)
{
    private IQueryable<T> Set<T>() where T : Tawtheef.Domain.Common.EventEntity =>
        unitOfWork.GetEntityRepository<T>().DbSet.AsNoTracking();

    // Every appointment (live or not) in scope. Live-only views add InterviewAppointment.IsLiveCandidate.
    public IQueryable<InterviewAppointment> Appointments(InterviewDashboardFilter filter, InterviewDashboardAccess access)
    {
        var query = Set<InterviewAppointment>().Where(a => !a.InterviewSchedule!.IsDeleted);

        if (access.ExecutionJobIds is not null)
        {
            var jobIds = access.ExecutionJobIds;
            query = query.Where(a => jobIds.Contains(a.InterviewSchedule!.JobId));
        }

        if (filter.JobId.HasValue)
            query = query.Where(a => a.InterviewSchedule!.JobId == filter.JobId.Value);
        if (filter.ScheduleId.HasValue)
            query = query.Where(a => a.InterviewScheduleId == filter.ScheduleId.Value);
        if (filter.CommitteeId.HasValue)
            query = query.Where(a => a.InterviewCommitteeId == filter.CommitteeId.Value);
        if (filter.InterviewType.HasValue)
            query = query.Where(a => a.InterviewType == filter.InterviewType.Value);
        if (filter.ScheduleStatus.HasValue)
            query = query.Where(a => a.InterviewSchedule!.Status == filter.ScheduleStatus.Value);
        if (filter.FromDate.HasValue)
        {
            var from = filter.FromDate.Value.ToDateTime(TimeOnly.MinValue).AddMinutes(-filter.UtcOffsetMinutes);
            query = query.Where(a => a.StartAt >= from);
        }
        if (filter.ToDate.HasValue)
        {
            var toExclusive = filter.ToDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue).AddMinutes(-filter.UtcOffsetMinutes);
            query = query.Where(a => a.StartAt < toExclusive);
        }
        // Result filters only apply for callers who may see results - otherwise toggling them would let an
        // execution-only user infer report states / final decisions from the execution counts.
        if (access.Results && filter.ResultReportStatus.HasValue)
        {
            var reports = Reports().Where(r => r.Status == filter.ResultReportStatus.Value);
            query = query.Where(a => reports.Any(r => r.InterviewScheduleId == a.InterviewScheduleId));
        }
        if (access.Results && filter.FinalDecision.HasValue)
        {
            var decided = ReportCandidates().Where(c => c.FinalDecision == filter.FinalDecision.Value);
            query = query.Where(a => decided.Any(c => c.InterviewAppointmentId == a.Id));
        }

        return query;
    }

    public IQueryable<InterviewAppointment> LiveAppointments(InterviewDashboardFilter filter, InterviewDashboardAccess access) =>
        Appointments(filter, access).Where(InterviewAppointment.IsLiveCandidate);

    // Schedule-level filters apply directly; appointment-only filters (dates, committee, type, decision)
    // keep a schedule only when at least one of its appointments matches.
    public IQueryable<InterviewSchedule> Schedules(InterviewDashboardFilter filter, InterviewDashboardAccess access)
    {
        var query = Set<InterviewSchedule>();

        if (access.ExecutionJobIds is not null)
        {
            var jobIds = access.ExecutionJobIds;
            query = query.Where(s => jobIds.Contains(s.JobId));
        }

        if (filter.JobId.HasValue)
            query = query.Where(s => s.JobId == filter.JobId.Value);
        if (filter.ScheduleId.HasValue)
            query = query.Where(s => s.Id == filter.ScheduleId.Value);
        if (filter.ScheduleStatus.HasValue)
            query = query.Where(s => s.Status == filter.ScheduleStatus.Value);
        if (access.Results && filter.ResultReportStatus.HasValue)
        {
            var reports = Reports().Where(r => r.Status == filter.ResultReportStatus.Value);
            query = query.Where(s => reports.Any(r => r.InterviewScheduleId == s.Id));
        }
        if (filter.HasAppointmentLevelFilter)
        {
            var matching = Appointments(filter, access);
            query = query.Where(s => matching.Any(a => a.InterviewScheduleId == s.Id));
        }

        return query;
    }

    // Result candidates of non-deleted reports, narrowed by the same filters through their appointment.
    // Results are not row-scoped (the result-report list isn't either), so no job restriction here.
    public IQueryable<InterviewResultCandidate> ResultCandidates(InterviewDashboardFilter filter)
    {
        var appointments = Appointments(filter, new InterviewDashboardAccess(false, false, true, null, true));
        return ReportCandidates().Where(c => appointments.Any(a => a.Id == c.InterviewAppointmentId));
    }

    public IQueryable<InterviewOperationalIssue> Issues(InterviewDashboardFilter filter, InterviewDashboardAccess access)
    {
        var appointments = Appointments(filter, access);
        return Set<InterviewOperationalIssue>().Where(i => appointments.Any(a => a.Id == i.InterviewAppointmentId));
    }

    // Setup entities only respond to the filters they can answer (job / committee / schedule).
    public IQueryable<InterviewCommittee> Committees(InterviewDashboardFilter filter)
    {
        var query = Set<InterviewCommittee>();
        if (filter.JobId.HasValue)
            query = query.Where(c => c.JobId == filter.JobId.Value);
        if (filter.CommitteeId.HasValue)
            query = query.Where(c => c.Id == filter.CommitteeId.Value);
        if (filter.ScheduleId.HasValue)
        {
            var scheduleAppointments = Set<InterviewAppointment>().Where(a => a.InterviewScheduleId == filter.ScheduleId.Value);
            query = query.Where(c => scheduleAppointments.Any(a => a.InterviewCommitteeId == c.Id));
        }
        return query;
    }

    public IQueryable<InterviewTemplate> Templates(InterviewDashboardFilter filter)
    {
        var query = Set<InterviewTemplate>().Where(t => t.IsActive);
        if (filter.JobId.HasValue || filter.CommitteeId.HasValue)
        {
            var committees = Committees(filter with { ScheduleId = null });
            query = query.Where(t => committees.Any(c => c.InterviewTemplateId == t.Id));
        }
        if (filter.ScheduleId.HasValue)
        {
            var schedules = Set<InterviewSchedule>().Where(s => s.Id == filter.ScheduleId.Value);
            query = query.Where(t => schedules.Any(s => s.InterviewTemplateId == t.Id));
        }
        return query;
    }

    public IQueryable<InterviewTemplateVersion> TemplateVersions() => Set<InterviewTemplateVersion>();

    public IQueryable<InterviewResultReport> Reports() => Set<InterviewResultReport>();

    public IQueryable<InterviewMemberEvaluation> MemberEvaluations() => Set<InterviewMemberEvaluation>();

    public IQueryable<InterviewCommitteeMember> CommitteeMembers() => Set<InterviewCommitteeMember>();

    public IQueryable<InterviewOperationalIssue> AllIssues() => Set<InterviewOperationalIssue>();

    // Candidates whose report was soft-deleted must not count - the report's own query filter doesn't
    // reach through the navigation, so the check is explicit.
    private IQueryable<InterviewResultCandidate> ReportCandidates() =>
        Set<InterviewResultCandidate>().Where(c => !c.InterviewResultReport!.IsDeleted);
}
