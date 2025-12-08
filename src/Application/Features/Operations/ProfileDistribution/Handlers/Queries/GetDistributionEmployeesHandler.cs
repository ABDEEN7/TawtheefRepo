using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;
using Tawtheef.Application.Features.Operations.ProfileDistribution.Queries;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionEmployeesHandler(IUnitOfWork uow)
    : IRequestHandler<GetDistributionEmployeesQuery, Result<IReadOnlyList<DistributionEmployeeDto>>>
{
    public async Task<Result<IReadOnlyList<DistributionEmployeeDto>>> Handle(
        GetDistributionEmployeesQuery request,
        CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow);
        var employees = await projection.LoadEmployeesAsync(ct);
        return Result.Ok<IReadOnlyList<DistributionEmployeeDto>>(employees);
    }
}
