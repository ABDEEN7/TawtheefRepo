using Application.Operation.Features.Admin.Users.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Users.Handlers.Commands;

public sealed class UpdateUserRolesCommandHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager)
    : ICommandHandler<UpdateUserRolesCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId && u.UserTypeId == UserTypeIds.Employee && !u.IsDeleted,
                cancellationToken);

        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        var roles = await roleManager.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        if (roles.Count != request.RoleIds.Count)
            return Result.Fail<Unit>(ErrorsCodes.RoleNotFound);

        var systemRoles = roles.Where(r => r.IsSystemRole).ToList();
        if (systemRoles.Count == 0)
            return Result.Fail<Unit>(ErrorsCodes.SystemRoleRequired);

        if (systemRoles.Count > 1)
            return Result.Fail<Unit>(ErrorsCodes.MultipleSystemRolesNotAllowed);

        var currentRoles = await userManager.GetRolesAsync(user);
        var systemRoleNames = await roleManager.Roles
            .Where(r => r.IsSystemRole && r.Name != null)
            .Select(r => r.Name!)
            .ToListAsync(cancellationToken);

        var currentSystemRoleName = currentRoles
            .FirstOrDefault(role => systemRoleNames.Contains(role, StringComparer.OrdinalIgnoreCase));
        var desiredSystemRoleName = systemRoles.FirstOrDefault()?.Name;

        if (string.IsNullOrWhiteSpace(desiredSystemRoleName))
            return Result.Fail<Unit>(ErrorsCodes.RoleNotFound);

        var lockedSystemRoles = new[]
        {
            nameof(SystemRoleIds.SystemAdmin),
            nameof(SystemRoleIds.OfficeAdmin),
            nameof(SystemRoleIds.OfficeUser)
        };

        if (lockedSystemRoles.Contains(desiredSystemRoleName, StringComparer.OrdinalIgnoreCase) &&
            !string.Equals(currentSystemRoleName, desiredSystemRoleName, StringComparison.OrdinalIgnoreCase))
            return Result.Fail<Unit>(ErrorsCodes.SystemRoleChangeNotAllowed);

        if (!string.IsNullOrWhiteSpace(currentSystemRoleName) &&
            lockedSystemRoles.Contains(currentSystemRoleName, StringComparer.OrdinalIgnoreCase) &&
            !string.Equals(currentSystemRoleName, desiredSystemRoleName, StringComparison.OrdinalIgnoreCase))
            return Result.Fail<Unit>(ErrorsCodes.SystemRoleChangeNotAllowed);

        var desiredRoles = roles
            .Select(r => r.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Cast<string>()
            .ToArray();

        var toRemove = currentRoles.Except(desiredRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        var toAdd = desiredRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToArray();

        if (toRemove.Length > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, toRemove);
            if (!removeResult.Succeeded)
                return FailureFromIdentity(removeResult);
        }

        if (toAdd.Length > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, toAdd);
            if (!addResult.Succeeded)
                return FailureFromIdentity(addResult);
        }

        return Result.Ok(Unit.Value);
    }

    private static Result<Unit> FailureFromIdentity(IdentityResult res)
        => Result.Fail<Unit>(string.Join(", ", res.Errors.Select(e => e.Description)));
}
