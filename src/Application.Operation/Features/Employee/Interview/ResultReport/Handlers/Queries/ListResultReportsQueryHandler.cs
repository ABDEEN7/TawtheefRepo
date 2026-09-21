using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using Application.Operation.Features.Employee.Interview.ResultReport.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Handlers.Queries;

public sealed class ListResultReportsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListResultReportsQuery, IResult<List<ResultReportListItemDto>>>
{
    public async Task<IResult<List<ResultReportListItemDto>>> Handle(ListResultReportsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet.AsNoTracking().AsQueryable();

        if (request.JobId.HasValue)
            query = query.Where(r => r.InterviewSchedule!.JobId == request.JobId.Value);
        if (request.Status.HasValue)
            query = query.Where(r => r.Status == request.Status.Value);

        var reports = await query
            .OrderByDescending(r => r.CreatedDate)
            .Select(r => new ResultReportListItemDto(
                r.Id,
                r.InterviewScheduleId,
                r.InterviewSchedule!.TitleAr,
                r.InterviewSchedule.TitleEn,
                r.InterviewSchedule.Job!.JobTitle!.JobNameAr,
                r.InterviewSchedule.Job.JobTitle.JobNameEn,
                r.Status,
                r.Candidates.Count,
                r.ApprovedAt))
            .ToListAsync(cancellationToken);

        return Result.Ok(reports);
    }
}
