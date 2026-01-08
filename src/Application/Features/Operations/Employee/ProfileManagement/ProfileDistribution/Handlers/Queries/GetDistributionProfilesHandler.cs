using Cortex.Mediator.Queries;
using FluentResults;

using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Queries;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionProfilesHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IQueryHandler<GetDistributionProfilesQuery, Result<PaginatedResult<DistributionProfileDto>>>
{
    public async Task<Result<PaginatedResult<DistributionProfileDto>>> Handle(
        GetDistributionProfilesQuery request,
        CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow, userManager);
        var items = await projection.LoadProfilesAsync(request, request.Status, ct);
        return Result.Ok(items);
    }
}
