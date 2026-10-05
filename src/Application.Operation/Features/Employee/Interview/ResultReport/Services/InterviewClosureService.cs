using System.Text.Json;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Services;

// System-triggered closing, called before SaveChanges from ApproveInterviewResultReport. The report
// itself stays Approved; its schedule and that schedule's live appointments close, then the committee
// closes too (BRD: Approved -> Closed once every linked interview is done) if this was its last
// unfinished schedule. Queries return the caller's tracked instances, so statuses are read in memory:
// the just-approved report and just-closed schedule still read as open in the DB.
public static class InterviewClosureService
{
    public static async Task<Result> CloseAfterReportApprovalAsync(
        IUnitOfWork unitOfWork, InterviewResultReport report, CancellationToken cancellationToken)
    {
        var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .Include(s => s.Appointments)
            .FirstOrDefaultAsync(s => s.Id == report.InterviewScheduleId, cancellationToken);
        if (schedule is null)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var scheduleResult = schedule.CloseOnResultApproved();
        if (scheduleResult.IsFailed)
            return scheduleResult;

        // One committee per schedule, but checked per distinct id so a mixed schedule can't slip through.
        var committeeIds = schedule.Appointments
            .Where(InterviewAppointment.IsLiveCandidateCompiled)
            .Select(a => a.InterviewCommitteeId)
            .Distinct();

        foreach (var committeeId in committeeIds)
        {
            var committeeResult = await TryCloseCommitteeIfDoneAsync(unitOfWork, committeeId, report.Id, cancellationToken);
            if (committeeResult.IsFailed)
                return committeeResult;
        }

        return Result.Ok();
    }

    // Done = every schedule the committee sits on (via its appointments) is Cancelled or Closed. Early
    // exits return Ok(): approval must never fail because the committee still has other schedules
    // running, or was stopped/closed manually in the meantime.
    private static async Task<Result> TryCloseCommitteeIfDoneAsync(
        IUnitOfWork unitOfWork, Guid committeeId, Guid triggeringReportId, CancellationToken cancellationToken)
    {
        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .FirstOrDefaultAsync(c => c.Id == committeeId, cancellationToken);
        if (committee is null || committee.Status != CommitteeStatus.Approved)
            return Result.Ok();

        var scheduleIds = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Where(a => a.InterviewCommitteeId == committeeId)
            .Select(a => a.InterviewScheduleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var schedules = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .Where(s => scheduleIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (schedules.Any(s => s.Status is not (ScheduleStatus.Cancelled or ScheduleStatus.Closed)))
            return Result.Ok();

        var closeResult = committee.Close();
        if (closeResult.IsFailed)
            return closeResult;

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewCommittee),
            EntityId = committee.Id,
            Action = InterviewCommitteeAuditActions.ClosedAutomatically,
            NewValues = JsonSerializer.Serialize(new { TriggeringReportId = triggeringReportId, ScheduleIds = scheduleIds })
        }, cancellationToken);

        return Result.Ok();
    }
}
