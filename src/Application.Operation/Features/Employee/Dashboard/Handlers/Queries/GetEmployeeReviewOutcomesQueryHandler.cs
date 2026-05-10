using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetEmployeeReviewOutcomesQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetEmployeeReviewOutcomesQuery, Result<EmployeeReviewOutcomesDto>>
{
    public Task<Result<EmployeeReviewOutcomesDto>> Handle(
        GetEmployeeReviewOutcomesQuery request,
        CancellationToken ct) => dashboardReadService.GetEmployeeReviewOutcomesAsync(request, ct);
}
