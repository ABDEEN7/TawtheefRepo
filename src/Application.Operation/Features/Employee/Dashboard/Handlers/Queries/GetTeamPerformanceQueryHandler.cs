using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetTeamPerformanceQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetTeamPerformanceQuery, Result<PaginatedResult<TeamPerformanceRowDto>>>
{
    public Task<Result<PaginatedResult<TeamPerformanceRowDto>>> Handle(
        GetTeamPerformanceQuery request,
        CancellationToken ct) => dashboardReadService.GetTeamPerformanceAsync(request, ct);
}
