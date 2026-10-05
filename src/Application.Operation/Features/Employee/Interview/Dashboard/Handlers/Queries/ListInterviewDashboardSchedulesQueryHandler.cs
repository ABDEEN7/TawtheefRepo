using Application.Operation.Features.Employee.Interview.Dashboard.DTOs;
using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using Application.Operation.Features.Employee.Interview.Dashboard.Services;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Handlers.Queries;

// Per-schedule execution report. Counts are schedule totals over its live appointments; the dashboard
// filters decide which schedules are listed. Report code/status only reach callers who may see results.
public sealed class ListInterviewDashboardSchedulesQueryHandler(
    InterviewDashboardAccessResolver accessResolver,
    InterviewDashboardQueryScope scope)
    : IRequestHandler<ListInterviewDashboardSchedulesQuery, IResult<PaginatedResult<InterviewDashboardScheduleRowDto>>>
{
    public async Task<IResult<PaginatedResult<InterviewDashboardScheduleRowDto>>> Handle(
        ListInterviewDashboardSchedulesQuery request, CancellationToken cancellationToken)
    {
        var access = await accessResolver.ResolveAsync(cancellationToken);
        if (!access.Execution)
            return InterviewDashboardPaging.Forbidden<InterviewDashboardScheduleRowDto>();

        var reports = scope.Reports();
        var openIssues = scope.AllIssues().Where(i => i.Status == OperationalIssueStatus.Open);

        var rows = scope.Schedules(request, access)
            .Select(s => new
            {
                s.Id,
                s.TitleAr,
                s.TitleEn,
                JobNameAr = s.Job!.JobTitle!.JobNameAr,
                JobNameEn = s.Job.JobTitle.JobNameEn,
                CommitteeNameAr = s.Appointments.Select(a => a.InterviewCommittee!.NameAr).FirstOrDefault(),
                CommitteeNameEn = s.Appointments.Select(a => a.InterviewCommittee!.NameEn).FirstOrDefault(),
                s.Status,
                First = s.Appointments.AsQueryable().Where(InterviewAppointment.IsLiveCandidate).Min(a => (DateTime?)a.StartAt),
                Last = s.Appointments.AsQueryable().Where(InterviewAppointment.IsLiveCandidate).Max(a => (DateTime?)a.StartAt),
                Candidates = s.Appointments.AsQueryable().Where(InterviewAppointment.IsLiveCandidate).Count(),
                Present = s.Appointments.AsQueryable().Where(InterviewAppointment.IsLiveCandidate)
                    .Count(a => a.AttendanceStatus == AttendanceStatus.Present),
                NoShow = s.Appointments.AsQueryable().Where(InterviewAppointment.IsLiveCandidate)
                    .Count(a => a.AttendanceStatus == AttendanceStatus.NoShow),
                EvaluationsCompleted = s.Appointments.AsQueryable().Where(InterviewAppointment.IsLiveCandidate)
                    .Count(a => a.Status == AppointmentStatus.Completed || a.Status == AppointmentStatus.Closed),
                OpenIssues = openIssues.Count(i => i.InterviewAppointment!.InterviewScheduleId == s.Id),
                ReportCode = reports.Where(r => r.InterviewScheduleId == s.Id).Select(r => r.Code).FirstOrDefault(),
                ReportStatus = reports.Where(r => r.InterviewScheduleId == s.Id).Select(r => (ResultReportStatus?)r.Status).FirstOrDefault()
            })
            .OrderByDescending(r => r.Last)
            .ThenBy(r => r.TitleAr);

        var includeReports = access.Results;
        return await InterviewDashboardPaging.ToPageAsync(rows, r => new InterviewDashboardScheduleRowDto(
            r.Id, r.TitleAr, r.TitleEn, r.JobNameAr, r.JobNameEn, r.CommitteeNameAr, r.CommitteeNameEn,
            r.Status, r.First.AsUtcOffset(), r.Last.AsUtcOffset(), r.Candidates, r.Present, r.NoShow, r.EvaluationsCompleted, r.OpenIssues,
            includeReports ? r.ReportCode : null,
            includeReports ? r.ReportStatus : null), request, cancellationToken);
    }
}
