using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Application.Operation.Features.Employee.Dashboard.Queries.Jobs;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Jobs;

internal sealed class GetLatestJobsQueryHandler(
    DashboardJobsReader reader)
    : IRequestHandler<GetLatestJobsQuery, Result<PaginatedResult<LatestJobDto>>>
{
    public Task<Result<PaginatedResult<LatestJobDto>>> Handle(
        GetLatestJobsQuery request,
        CancellationToken ct) =>
        reader.ReadLatestAsync(request, ct);
}
