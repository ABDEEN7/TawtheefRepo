using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using Application.Operation.Features.Employee.Dashboard.Queries.Overview;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Overview;

internal sealed class GetDashboardOverviewQueryHandler(DashboardOverviewReader reader)
    : IRequestHandler<GetDashboardOverviewQuery, Result<DashboardOverviewDto>>
{
    public Task<Result<DashboardOverviewDto>> Handle(GetDashboardOverviewQuery request, CancellationToken ct) =>
        reader.ReadAsync(request, ct);
}
