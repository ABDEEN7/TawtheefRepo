using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Users.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Handlers.Commands;

public sealed class UpdateUserRolesCommandHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<UpdateUserRolesCommand, IResult<Unit>>
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

        var currentRoles = await userManager.GetRolesAsync(user);
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
