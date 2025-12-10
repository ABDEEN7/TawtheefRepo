using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Users.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Handlers.Queries;

public sealed class GetUserRolesQueryHandler(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IMapper mapper)
    : IRequestHandler<GetUserRolesQuery, IResult<UserRoleAssignmentDto>>
{
    public async Task<IResult<UserRoleAssignmentDto>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users.AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId &&
                     u.UserTypeId == UserTypeIds.Employee &&
                     !u.IsDeleted,
                cancellationToken);

        if (user is null)
            return Result.Fail<UserRoleAssignmentDto>(ErrorsCodes.UserNotFound);

        var roleEntities = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        var roleSummaries = new List<RoleSummaryDto>(roleEntities.Count);
        foreach (var role in roleEntities)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            roleSummaries.Add(mapper.Map<RoleSummaryDto>(new RoleWithClaims(role, claims)));
        }

        var assignedRoleNames = await userManager.GetRolesAsync(user);
        var assignedRoleIds = roleEntities
            .Where(r => assignedRoleNames.Contains(r.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            .Select(r => r.Id)
            .ToArray();

        var dto = mapper.Map<UserRoleAssignmentDto>(user) with
        {
            Roles = roleSummaries,
            AssignedRoleIds = assignedRoleIds
        };

        return Result.Ok(dto);
    }
}
