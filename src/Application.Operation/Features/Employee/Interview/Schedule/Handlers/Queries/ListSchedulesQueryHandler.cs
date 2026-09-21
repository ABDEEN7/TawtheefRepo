using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
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
                s.DefaultInterviewType,
                CandidatesCount = s.Appointments.Count(a =>
                    a.InvitationId != null && a.Status != AppointmentStatus.Rescheduled && a.Status != AppointmentStatus.Cancelled),
                s.Status
            })
            .ToListAsync(cancellationToken);

        // One active committee per job (unique index), so a single lookup by job id resolves every row's committee.
        var jobIds = raw.Select(s => s.JobId).Distinct().ToList();
        var committeeByJob = (await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
                .AsNoTracking()
                .Where(c => jobIds.Contains(c.JobId) && c.IsActive)
                .Select(c => new { c.JobId, c.NameAr, c.NameEn })
                .ToListAsync(cancellationToken))
            .GroupBy(c => c.JobId)
            .ToDictionary(g => g.Key, g => g.First());

        var result = raw.Select(s =>
        {
            var hasSlots = s.Slots.Count > 0;
            committeeByJob.TryGetValue(s.JobId, out var committee);

            return new ScheduleListItemDto(
                s.Id,
                s.JobId,
                s.JobTitleId,
                s.JobTitleNameAr,
                s.JobTitleNameEn,
                committee?.NameAr,
                committee?.NameEn,
                hasSlots ? DateOnly.FromDateTime(s.Slots.Min(x => x.StartAt)) : null,
                hasSlots ? DateOnly.FromDateTime(s.Slots.Max(x => x.StartAt)) : null,
                hasSlots ? s.Slots.Min(x => TimeOnly.FromDateTime(x.StartAt)) : null,
                hasSlots ? s.Slots.Max(x => TimeOnly.FromDateTime(x.EndAt)) : null,
                s.DefaultInterviewType,
                s.CandidatesCount,
                s.Status);
        }).ToList();

        return Result.Ok(result);
    }
}
