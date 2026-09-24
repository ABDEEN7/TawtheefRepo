using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using Application.Operation.Features.Employee.Interview.ResultReport.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Handlers.Queries;

public sealed class ListResultReportsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListResultReportsQuery, IResult<List<ResultReportListItemDto>>>
{
    public async Task<IResult<List<ResultReportListItemDto>>> Handle(ListResultReportsQuery request, CancellationToken cancellationToken)
    {
        // Discarded reports are soft-deleted, so the soft-delete filter is lifted here - but only to bring
        // back the ones discarded by a reschedule (identified by their audit row); anything deleted for
        // another reason stays hidden, and so does anything on a deleted schedule.
        var discards = unitOfWork.GetEntityRepository<InterviewAuditLog>().DbSet.AsNoTracking()
            .Where(l => l.EntityType == nameof(InterviewResultReport) && l.Action == InterviewResultReportAuditActions.Discarded);

        var query = unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet.AsNoTracking()
            .IgnoreQueryFilters()
            .Where(r => !r.InterviewSchedule!.IsDeleted
                && (!r.IsDeleted || discards.Any(l => l.EntityId == r.Id)));

        if (request.JobId.HasValue)
            query = query.Where(r => r.InterviewSchedule!.JobId == request.JobId.Value);
        // A status filter is about live reports; a discarded one keeps its last status but isn't "in" it.
        if (request.Status.HasValue)
            query = query.Where(r => !r.IsDeleted && r.Status == request.Status.Value);

        var reports = await query
            .OrderByDescending(r => r.CreatedDate)
            .Select(r => new ResultReportListItemDto(
                r.Id,
                r.Code,
                r.InterviewScheduleId,
                r.InterviewSchedule!.TitleAr,
                r.InterviewSchedule.TitleEn,
                r.InterviewSchedule.Job!.JobTitle!.JobNameAr,
                r.InterviewSchedule.Job.JobTitle.JobNameEn,
                r.AppliedQualificationScore,
                r.Status,
                r.Candidates.Count,
                r.ApprovedAt.AsUtcOffset(),
                r.IsDeleted,
                r.DeletedDate.AsUtcOffset(),
                discards.Where(l => l.EntityId == r.Id).OrderByDescending(l => l.CreatedDate).Select(l => l.Reason).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return Result.Ok(reports);
    }
}
