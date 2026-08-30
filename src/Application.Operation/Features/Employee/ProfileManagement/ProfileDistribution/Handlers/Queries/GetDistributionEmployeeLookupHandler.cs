using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionEmployeeLookupHandler(ProfileDistributionProjection projection)
    : IRequestHandler<GetDistributionEmployeeLookupQuery, Result<IReadOnlyList<DistributionEmployeeLookupDto>>>
{
    public async Task<Result<IReadOnlyList<DistributionEmployeeLookupDto>>> Handle(
        GetDistributionEmployeeLookupQuery request,
        CancellationToken ct)
    {
        var employees = await projection.LoadEmployeeLookupAsync(request.UserId, ct);
        return Result.Ok(employees);
    }
}
