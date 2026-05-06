using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetDashboardOverviewQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetDashboardOverviewQuery, Result<DashboardOverviewDto>>
{
    public Task<Result<DashboardOverviewDto>> Handle(GetDashboardOverviewQuery request, CancellationToken ct) =>
        dashboardReadService.GetOverviewAsync(request, ct);
}
