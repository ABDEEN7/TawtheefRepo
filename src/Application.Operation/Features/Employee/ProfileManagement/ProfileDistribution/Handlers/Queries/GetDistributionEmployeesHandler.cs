using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionEmployeesHandler(ProfileDistributionProjection projection)
    : IRequestHandler<GetDistributionEmployeesQuery, Result<PaginatedResult<DistributionEmployeeDto>>>
{
    public async Task<Result<PaginatedResult<DistributionEmployeeDto>>> Handle(
        GetDistributionEmployeesQuery request,
        CancellationToken ct)
    {
        var employees = await projection.LoadEmployeesAsync(request, ct);
        return Result.Ok(employees);
    }
}

