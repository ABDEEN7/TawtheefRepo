using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;
using Tawtheef.Application.Features.Operations.ProfileDistribution.Queries;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionProfilesHandler(IUnitOfWork uow)
    : IRequestHandler<GetDistributionProfilesQuery, Result<IReadOnlyList<DistributionProfileDto>>>
{
    public async Task<Result<IReadOnlyList<DistributionProfileDto>>> Handle(
        GetDistributionProfilesQuery request,
        CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow);
        var items = await projection.LoadProfilesAsync(request.Status, ct);
        return Result.Ok<IReadOnlyList<DistributionProfileDto>>(items);
    }
}
