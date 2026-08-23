using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.Queries.Employees;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Employees;

internal sealed class GetTeamPerformanceQueryHandler(DashboardEmployeesReader reader)
    : IRequestHandler<GetTeamPerformanceQuery, Result<PaginatedResult<TeamPerformanceRowDto>>>
{
    public Task<Result<PaginatedResult<TeamPerformanceRowDto>>> Handle(GetTeamPerformanceQuery request, CancellationToken ct) =>
        reader.ReadAsync(request, ct);
}
