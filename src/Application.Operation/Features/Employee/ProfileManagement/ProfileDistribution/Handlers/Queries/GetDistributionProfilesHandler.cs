using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Users;
using MapsterMapper;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionProfilesHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IUserRepository userRepository,
    ILocalizationService localizationService,
    IMapper mapper)
    : IRequestHandler<GetDistributionProfilesQuery, Result<PaginatedResult<DistributionProfileDto>>>
{
    public async Task<Result<PaginatedResult<DistributionProfileDto>>> Handle(
        GetDistributionProfilesQuery request,
        CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow, userManager, userRepository, localizationService, mapper);
        var items = await projection.LoadProfilesAsync(
            request.UserId!.Value,
            request,
            request.Status,
            request.SearchTerm,
            request.TargetEntityId,
            ct);
        return Result.Ok(items);
    }
}

