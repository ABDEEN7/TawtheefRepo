using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileDistribution.Queries;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileDistribution.Handlers.Queries;

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
