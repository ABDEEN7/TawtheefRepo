using Application.Operation.Features.Admin.Users.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Users.Handlers.Queries;

public sealed class GetUserAssignedRoleIdsQueryHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IQueryHandler<GetUserAssignedRoleIdsQuery, IResult<IReadOnlyCollection<Guid>>>
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
            return Result.Ok<IReadOnlyCollection<Guid>>([]);

        var roleNameSet = new HashSet<string>(assignedRoleNames, StringComparer.OrdinalIgnoreCase);

        var roleIds = await roleManager.Roles
            .AsNoTracking()
            .Where(role => role.Name != null && roleNameSet.Contains(role.Name))
            .Select(role => role.Id)
            .ToArrayAsync(cancellationToken);

        return Result.Ok<IReadOnlyCollection<Guid>>(roleIds);
    }
}
