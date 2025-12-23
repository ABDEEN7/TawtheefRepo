using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Queries;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionEmployeesHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IRequestHandler<GetDistributionEmployeesQuery, Result<IReadOnlyList<DistributionEmployeeDto>>>
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
