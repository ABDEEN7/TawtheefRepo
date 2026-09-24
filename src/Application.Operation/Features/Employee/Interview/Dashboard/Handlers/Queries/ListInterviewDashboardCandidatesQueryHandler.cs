using Application.Operation.Features.Employee.Interview.Dashboard.DTOs;
using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using Application.Operation.Features.Employee.Interview.Dashboard.Services;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Handlers.Queries;

// Candidate attendance / execution / evaluation-progress report over live appointments.
// RequiredEvaluations uses the same quorum membership rule as SubmitMemberEvaluationCommandHandler
// (active members who participate in evaluation); only counts are exposed, never who scored what.
public sealed class ListInterviewDashboardCandidatesQueryHandler(
    InterviewDashboardAccessResolver accessResolver,
    InterviewDashboardQueryScope scope)
    : IRequestHandler<ListInterviewDashboardCandidatesQuery, IResult<PaginatedResult<InterviewDashboardCandidateRowDto>>>
{
    public async Task<IResult<PaginatedResult<InterviewDashboardCandidateRowDto>>> Handle(
        ListInterviewDashboardCandidatesQuery request, CancellationToken cancellationToken)
    {
        var access = await accessResolver.ResolveAsync(cancellationToken);
        if (!access.Execution)
            return InterviewDashboardPaging.Forbidden<InterviewDashboardCandidateRowDto>();

        var query = scope.LiveAppointments(request, access);
        query = request.Attendance switch
        {
            InterviewDashboardAttendanceView.Present => query.Where(a => a.AttendanceStatus == AttendanceStatus.Present),
            InterviewDashboardAttendanceView.NoShow => query.Where(a => a.AttendanceStatus == AttendanceStatus.NoShow),
            InterviewDashboardAttendanceView.Withdrew => query.Where(a => a.AttendanceStatus == AttendanceStatus.Withdrew),
            InterviewDashboardAttendanceView.Late => query.Where(a => a.AttendanceStatus == AttendanceStatus.Late),
            InterviewDashboardAttendanceView.NotRecorded => query.Where(a => a.AttendanceStatus == null),
            _ => query
        };

        var submitted = scope.MemberEvaluations().Where(e => e.Status == MemberEvaluationStatus.Submitted);
        var quorumMembers = scope.CommitteeMembers().Where(m => m.IsActive && m.ParticipatesInEvaluation);

        var rows = query
            .Select(a => new
            {
                a.Id,
                a.InterviewScheduleId,
                CandidateNameAr = a.Invitation!.Applicant!.FullNameAr,
                CandidateNameEn = a.Invitation.Applicant.FullNameEn,
                JobNameAr = a.InterviewSchedule!.Job!.JobTitle!.JobNameAr,
                JobNameEn = a.InterviewSchedule.Job.JobTitle.JobNameEn,
                ScheduleTitleAr = a.InterviewSchedule.TitleAr,
                ScheduleTitleEn = a.InterviewSchedule.TitleEn,
                a.StartAt,
                a.InterviewType,
                a.Status,
                a.AttendanceStatus,
                Submitted = submitted.Count(e => e.InterviewAppointmentId == a.Id),
                Required = quorumMembers.Count(m => m.InterviewCommitteeId == a.InterviewCommitteeId)
            })
            .OrderByDescending(r => r.StartAt)
            .ThenBy(r => r.Id);

        return await InterviewDashboardPaging.ToPageAsync(rows, r => new InterviewDashboardCandidateRowDto(
            r.Id, r.InterviewScheduleId, r.CandidateNameAr, r.CandidateNameEn, r.JobNameAr, r.JobNameEn,
            r.ScheduleTitleAr, r.ScheduleTitleEn, r.StartAt.AsUtcOffset(), r.InterviewType, r.Status, r.AttendanceStatus,
            r.Submitted, r.Required), request, cancellationToken);
    }
}
