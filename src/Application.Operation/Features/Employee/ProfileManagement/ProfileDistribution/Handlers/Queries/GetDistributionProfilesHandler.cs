using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionProfilesHandler(ProfileDistributionProjection projection)
    : IRequestHandler<GetDistributionProfilesQuery, Result<PaginatedResult<DistributionProfileDto>>>
{
    public async Task<Result<PaginatedResult<DistributionProfileDto>>> Handle(
        GetDistributionProfilesQuery request,
        CancellationToken ct)
    {
        var items = await projection.LoadProfilesAsync(
            request.UserId!.Value,
            request,
            ct);
        return Result.Ok(items);
    }
}

