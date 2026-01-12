using Application.Operation.Features.Employee.JobInvitationSummary.DTOs;
using Application.Operation.Features.Employee.JobInvitationSummary.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;

namespace Application.Operation.Features.Employee.JobInvitationSummary.Handlers;

public sealed class GetJobInvitationSummaryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetJobInvitationSummaryQuery, IResult<PaginatedResult<JobInvitationSummaryDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDto>>> Handle(GetJobInvitationSummaryQuery query, CancellationToken cancellationToken)
    {
        var invitations =  await unitOfWork.GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(i => i.JobCategory)
            .Include(i => i.Invitations)
            .Include(i => i.Department)
            .Include(i => i.JobStatus)
            .WhereIf(query.JobCategoryId is not null, i => i.JobCategoryId == query.JobCategoryId)
            .WhereIf(query.DepartmentId is not null, i => i.DepartmentId == query.DepartmentId)
            .WhereIf(query.JobStatusId is not null, i => i.JobStatusId == query.JobStatusId)
            .ToPaginatedListAsync<Tawtheef.Domain.Entities.Recruitment.Job, JobInvitationSummaryDto>(mapper, query, cancellationToken);

        return Result.Ok(invitations);
    }
}
