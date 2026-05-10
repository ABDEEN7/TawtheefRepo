using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetEmployeeIndicatorsQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetEmployeeIndicatorsQuery, Result<EmployeeIndicatorsDto>>
{
    public Task<Result<EmployeeIndicatorsDto>> Handle(GetEmployeeIndicatorsQuery request, CancellationToken ct) =>
        dashboardReadService.GetEmployeeIndicatorsAsync(request, ct);
}
