using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionEmployeesHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IQueryHandler<GetDistributionEmployeesQuery, Result<IReadOnlyList<DistributionEmployeeDto>>>
{
    public async Task<Result<IReadOnlyList<DistributionEmployeeDto>>> Handle(
        GetDistributionEmployeesQuery request,
        CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow,userManager);
        var employees = await projection.LoadEmployeesAsync(ct);
        return Result.Ok(employees);
    }
}
