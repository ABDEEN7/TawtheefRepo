using Application.Operation.Features.Admin.Roles.DTOs;
using Application.Operation.Features.Admin.Users.DTOs;
using Application.Operation.Features.Admin.Users.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Users.Handlers.Queries;

public sealed class GetUserRolesQueryHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper)
    : IQueryHandler<GetUserRolesQuery, IResult<UserRoleAssignmentDto>>
{
    public async Task<IResult<UserRoleAssignmentDto>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

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
