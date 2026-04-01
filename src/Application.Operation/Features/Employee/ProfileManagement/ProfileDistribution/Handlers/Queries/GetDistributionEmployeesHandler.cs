using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Queries;

public sealed class GetDistributionEmployeesHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IUserRepository userRepository,
    ILocalizationService localizationService,
    IMapper mapper)
    : IRequestHandler<GetDistributionEmployeesQuery, Result<IReadOnlyList<DistributionEmployeeDto>>>
{
    public async Task<Result<IReadOnlyList<DistributionEmployeeDto>>> Handle(
        GetDistributionEmployeesQuery request,
        CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow, userManager, userRepository, localizationService, mapper);
        var employees = await projection.LoadEmployeesAsync(request.UserId, ct);
        return Result.Ok(employees);
    }
}

