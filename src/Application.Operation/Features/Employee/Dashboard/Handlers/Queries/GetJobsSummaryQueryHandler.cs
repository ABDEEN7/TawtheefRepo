using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetJobsSummaryQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetJobsSummaryQuery, Result<JobsSummaryDto>>
{
    public Task<Result<JobsSummaryDto>> Handle(GetJobsSummaryQuery request, CancellationToken ct) =>
        dashboardReadService.GetJobsSummaryAsync(request, ct);
}
