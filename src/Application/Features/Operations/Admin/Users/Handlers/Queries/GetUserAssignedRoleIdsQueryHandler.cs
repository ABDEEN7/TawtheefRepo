using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Users.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Handlers.Queries;

public sealed class GetUserAssignedRoleIdsQueryHandler(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<GetUserAssignedRoleIdsQuery, IResult<IReadOnlyCollection<Guid>>>
{
    public async Task<IResult<IReadOnlyCollection<Guid>>> Handle(
        GetUserAssignedRoleIdsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            return Result.Fail<IReadOnlyCollection<Guid>>(ErrorsCodes.UserNotFound);

        var assignedRoleNames = await userManager.GetRolesAsync(user);

        if (assignedRoleNames.Count == 0)
            return Result.Ok<IReadOnlyCollection<Guid>>(Array.Empty<Guid>());

        var roleNameSet = new HashSet<string>(assignedRoleNames, StringComparer.OrdinalIgnoreCase);

        var roleIds = await roleManager.Roles
            .AsNoTracking()
            .Where(role => role.Name != null && roleNameSet.Contains(role.Name))
            .Select(role => role.Id)
            .ToArrayAsync(cancellationToken);

        return Result.Ok<IReadOnlyCollection<Guid>>(roleIds);
    }
}
