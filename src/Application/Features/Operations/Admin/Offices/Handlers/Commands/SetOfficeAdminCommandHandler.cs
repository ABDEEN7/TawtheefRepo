using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class SetOfficeAdminCommandHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<SetOfficeAdminCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SetOfficeAdminCommand request, CancellationToken cancellationToken)
    {
        var targetUser = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId &&
                     u.OfficeId == request.OfficeId &&
                     !u.IsDeleted,
                cancellationToken);

        if (targetUser is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeUserNotFound);

        var adminRole = await roleManager.FindByNameAsync(SystemRoles.OfficeAdmin);
        if (adminRole is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeAdminRoleNotFound);

        var officeUserRole = await roleManager.FindByNameAsync(SystemRoles.OfficeUser);
        if (officeUserRole is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeUserRoleNotFound);

        var currentAdmins = await userManager.GetUsersInRoleAsync(SystemRoles.OfficeAdmin);
        var officeAdmins = currentAdmins
            .OfType<OfficeUser>()
            .Where(u => u.OfficeId == request.OfficeId && !u.IsDeleted)
            .ToList();

        foreach (var admin in officeAdmins.Where(admin => admin.Id != targetUser.Id))
        {
            var removeResult = await userManager.RemoveFromRoleAsync(admin, SystemRoles.OfficeAdmin);
            if (!removeResult.Succeeded)
                return FailureFromIdentity(removeResult);

            if (!await userManager.IsInRoleAsync(admin, SystemRoles.OfficeUser))
            {
                var addToUserRole = await userManager.AddToRoleAsync(admin, SystemRoles.OfficeUser);
                if (!addToUserRole.Succeeded)
                    return FailureFromIdentity(addToUserRole);
            }
        }

        if (!await userManager.IsInRoleAsync(targetUser, SystemRoles.OfficeAdmin))
        {
            var addResult = await userManager.AddToRoleAsync(targetUser, SystemRoles.OfficeAdmin);
            if (!addResult.Succeeded)
                return FailureFromIdentity(addResult);
        }

        return Result.Ok(Unit.Value);
    }

    private static Result<Unit> FailureFromIdentity(IdentityResult res)
        => Result.Fail<Unit>(string.Join(", ", res.Errors.Select(e => e.Description)));
}
