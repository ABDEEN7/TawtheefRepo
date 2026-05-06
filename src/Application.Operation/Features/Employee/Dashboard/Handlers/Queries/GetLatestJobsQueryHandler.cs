using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetLatestJobsQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetLatestJobsQuery, Result<IReadOnlyList<LatestJobDto>>>
{
    public Task<Result<IReadOnlyList<LatestJobDto>>> Handle(GetLatestJobsQuery request, CancellationToken ct) =>
        dashboardReadService.GetLatestJobsAsync(request, ct);
}
