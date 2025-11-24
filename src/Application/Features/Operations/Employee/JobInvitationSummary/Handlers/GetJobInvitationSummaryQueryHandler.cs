using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.Queries;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.Handlers;

public sealed class GetJobInvitationSummaryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetJobInvitationSummaryQuery, IResult<PaginatedResult<JobInvitationSummaryDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDto>>> Handle(GetJobInvitationSummaryQuery query, CancellationToken cancellationToken)
    {
        var invitations =  await unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .Include(i => i.JobCategory)
            .Include(i => i.Invitations)
            .Include(i => i.RequestingDepartment)
            .WhereIf(query.JobCategoryId is not null, i => i.JobCategoryId == query.JobCategoryId)
            .WhereIf(query.DepartmentId is not null, i => i.RequestingDepartmentId == query.DepartmentId)
            .WhereIf(query.JobStatusId is not null, i => i.StatusId == query.JobStatusId)
            .ToPaginatedListAsync<Job, JobInvitationSummaryDto>(mapper, query, cancellationToken);

        return Result.Ok(invitations);
    }
}
