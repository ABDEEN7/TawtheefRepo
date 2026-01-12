using Application.Operation.Features.Admin.Roles.DTOs;
using Application.Operation.Features.Admin.Roles.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.Handlers.Queries;

public sealed class GetRoleDetailsQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    : IQueryHandler<GetRoleDetailsQuery, IResult<RoleDto>>
{
    public async Task<IResult<RoleDto>> Handle(GetRoleDetailsQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (role is null)
            return Result.Fail<RoleDto>(ErrorsCodes.RoleNotFound);

        var claims = await roleManager.GetClaimsAsync(role);
        return Result.Ok(mapper.Map<RoleDto>(new RoleWithClaims(role, claims)));
    }
}
