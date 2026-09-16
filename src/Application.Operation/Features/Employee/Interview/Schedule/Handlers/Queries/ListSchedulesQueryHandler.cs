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
                JobTitleNameAr = s.Job != null && s.Job.JobTitle != null ? s.Job.JobTitle.JobNameAr : null,
                JobTitleNameEn = s.Job != null && s.Job.JobTitle != null ? s.Job.JobTitle.JobNameEn : null,
                FirstStartAt = s.Appointments.OrderBy(a => a.StartAt).Select(a => (DateTime?)a.StartAt).FirstOrDefault(),
                s.DefaultInterviewType,
                CandidatesCount = s.Appointments.Count(a =>
                    a.InvitationId != null && a.Status != AppointmentStatus.Rescheduled && a.Status != AppointmentStatus.Cancelled),
                s.Status
            })
            .ToListAsync(cancellationToken);

        var result = raw.Select(s => new ScheduleListItemDto(
            s.Id,
            s.JobId,
            s.JobTitleNameAr,
            s.JobTitleNameEn,
            s.FirstStartAt.HasValue ? DateOnly.FromDateTime(s.FirstStartAt.Value) : null,
            s.FirstStartAt.HasValue ? TimeOnly.FromDateTime(s.FirstStartAt.Value) : null,
            s.DefaultInterviewType,
            s.CandidatesCount,
            s.Status)).ToList();

        return Result.Ok(result);
    }
}
