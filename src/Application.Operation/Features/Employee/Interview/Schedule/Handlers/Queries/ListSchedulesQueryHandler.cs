using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Queries;

public sealed class ListSchedulesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListSchedulesQuery, IResult<List<ScheduleListItemDto>>>
{
    public async Task<IResult<List<ScheduleListItemDto>>> Handle(ListSchedulesQuery request, CancellationToken cancellationToken)
    {
        var raw = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .AsNoTracking()
            .WhereIf(request.JobId.HasValue, s => s.JobId == request.JobId!.Value)
            .WhereIf(request.Status.HasValue, s => s.Status == request.Status!.Value)
            .OrderByDescending(s => s.CreatedDate)
            .Select(s => new
            {
                s.Id,
                s.JobId,
                JobTitleId = s.Job != null ? (Guid?)s.Job.JobTitleId : null,
                JobTitleNameAr = s.Job != null && s.Job.JobTitle != null ? s.Job.JobTitle.JobNameAr : null,
                JobTitleNameEn = s.Job != null && s.Job.JobTitle != null ? s.Job.JobTitle.JobNameEn : null,
                // Live slots only (superseded/cancelled rows are no longer part of the schedule) - the same rule
                // CandidatesCount uses below.
                Slots = s.Appointments
                    .Where(a => a.Status != AppointmentStatus.Rescheduled && a.Status != AppointmentStatus.Cancelled)
                    .Select(a => new { a.StartAt, a.EndAt })
                    .ToList(),
                // The committee that runs this schedule lives on its appointments (all share it).
                CommitteeId = s.Appointments.Select(a => (Guid?)a.InterviewCommitteeId).FirstOrDefault(),
                s.DefaultBufferMinutes,
                s.DefaultInterviewType,
                CandidatesCount = s.Appointments.Count(a =>
                    a.InvitationId != null && a.Status != AppointmentStatus.Rescheduled && a.Status != AppointmentStatus.Cancelled),
                s.Status
            })
            .ToListAsync(cancellationToken);

        // A job can outlive its committee (closed/cancelled -> a new one takes the job), so each row shows the
        // committee on its own appointments. Only a schedule with no appointments falls back to the job's
        // current (active) committee - the one it would be created with.
        var committeeIds = raw.Where(s => s.CommitteeId.HasValue).Select(s => s.CommitteeId!.Value).Distinct().ToList();
        var fallbackJobIds = raw.Where(s => !s.CommitteeId.HasValue).Select(s => s.JobId).Distinct().ToList();
        var committees = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .Where(c => committeeIds.Contains(c.Id) || (fallbackJobIds.Contains(c.JobId) && c.IsActive))
            .Select(c => new { c.Id, c.JobId, c.IsActive, c.NameAr, c.NameEn })
            .ToListAsync(cancellationToken);
        var committeeById = committees.ToDictionary(c => c.Id);
        var activeCommitteeByJob = committees
            .Where(c => c.IsActive)
            .GroupBy(c => c.JobId)
            .ToDictionary(g => g.Key, g => g.First());

        var result = raw.Select(s =>
        {
            var sessions = ScheduleAppointmentPlanner
                .ReconstructPeriods(s.Slots.Select(x => new GeneratedSlotDto(x.StartAt, x.EndAt, null, null, null)), s.DefaultBufferMinutes)
                .Select(p => new ScheduleSessionDto(p.StartAt, p.EndAt))
                .ToList();
            var committee = s.CommitteeId.HasValue
                ? committeeById.GetValueOrDefault(s.CommitteeId.Value)
                : activeCommitteeByJob.GetValueOrDefault(s.JobId);

            return new ScheduleListItemDto(
                s.Id,
                s.JobId,
                s.JobTitleId,
                s.JobTitleNameAr,
                s.JobTitleNameEn,
                committee?.NameAr,
                committee?.NameEn,
                sessions,
                s.DefaultInterviewType,
                s.CandidatesCount,
                s.Status);
        }).ToList();

        return Result.Ok(result);
    }
}
