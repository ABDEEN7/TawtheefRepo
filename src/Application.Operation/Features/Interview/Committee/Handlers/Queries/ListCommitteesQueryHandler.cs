using Application.Operation.Features.Interview.Committee.DTOs;
using Application.Operation.Features.Interview.Committee.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.Committee.Handlers.Queries;

public sealed class ListCommitteesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListCommitteesQuery, IResult<List<CommitteeDto>>>
{
    public async Task<IResult<List<CommitteeDto>>> Handle(ListCommitteesQuery request, CancellationToken cancellationToken)
    {
        var committees = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .WhereIf(request.JobId.HasValue, c => c.JobId == request.JobId!.Value)
            .OrderByDescending(c => c.CreatedDate)
            .Select(c => new CommitteeDto(
                c.Id,
                c.JobId,
                c.Job != null && c.Job.JobTitle != null ? c.Job.JobTitle.JobNameAr : null,
                c.Job != null && c.Job.JobTitle != null ? c.Job.JobTitle.JobNameEn : null,
                c.InterviewTemplateId,
                c.InterviewTemplate!.TitleAr,
                c.InterviewTemplate.TitleEn,
                c.Job!.JobCategoryId,
                c.Job.JobCategory!.NameAr,
                c.Job.JobCategory.NameEn,
                c.NameAr,
                c.NameEn,
                c.ScopeDescription,
                c.Notes,
                c.Status,
                c.IsActive,
                c.ApprovedById,
                c.ApprovedAt,
                c.ClosedAt,
                c.DecisionNotes))
            .ToListAsync(cancellationToken);

        return Result.Ok(committees);
    }
}
