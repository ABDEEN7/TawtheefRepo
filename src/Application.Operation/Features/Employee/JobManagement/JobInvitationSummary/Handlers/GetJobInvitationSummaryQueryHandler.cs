using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Services;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Handlers;

internal sealed class GetJobInvitationSummaryQueryHandler(JobInvitationSummaryQueryBuilder queryBuilder)
    : IRequestHandler<GetJobInvitationSummaryQuery, IResult<PaginatedResult<JobInvitationSummaryDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDto>>> Handle(GetJobInvitationSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var queryable = queryBuilder.ApplySorting(
            queryBuilder.Build(query), query.SortBy, query.SortDirection);
        var pagination = query with { SortBy = null, SortDirection = null };
        var result = await queryable.ToPaginatedListAsync(pagination, cancellationToken);

        return Result.Ok(result);
    }
}

