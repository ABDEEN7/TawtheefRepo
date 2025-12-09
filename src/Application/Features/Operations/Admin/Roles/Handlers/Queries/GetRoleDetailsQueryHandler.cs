using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Queries;

public sealed class GetRoleDetailsQueryHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<GetRoleDetailsQuery, IResult<RoleDto>>
{
    public async Task<IResult<RoleDto>> Handle(GetRoleDetailsQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (role is null)
            return Result.Fail<RoleDto>(ErrorsCodes.RoleNotFound);

        var claims = await roleManager.GetClaimsAsync(role);
        return Result.Ok(RoleMapping.ToDto(role, claims));
    }
}
