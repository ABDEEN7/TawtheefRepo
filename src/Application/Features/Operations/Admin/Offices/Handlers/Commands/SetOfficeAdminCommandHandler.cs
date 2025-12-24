using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class SetOfficeAdminCommandHandler(
    UserManager<User> userManager)
    : IRequestHandler<SetOfficeAdminCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        SetOfficeAdminCommand request,
        CancellationToken cancellationToken)
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

        // Get current OfficeAdmins in this office
        var currentAdmins = (await userManager
                .GetUsersInRoleAsync(nameof(SystemRoleIds.OfficeAdmin)))
            .OfType<OfficeUser>()
            .Where(u => u.OfficeId == request.OfficeId && !u.IsDeleted)
            .ToList();

        // 1️⃣ Demote old admins
        foreach (var admin in currentAdmins.Where(a => a.Id != targetUser.Id))
        {
            var removeAdmin = await userManager.RemoveFromRoleAsync(
                admin,
                nameof(SystemRoleIds.OfficeAdmin));

            if (!removeAdmin.Succeeded)
                return FailureFromIdentity(removeAdmin);

            // ensure OfficeUser role
            if (!await userManager.IsInRoleAsync(admin, nameof(SystemRoleIds.OfficeUser)))
            {
                var addUser = await userManager.AddToRoleAsync(
                    admin,
                    nameof(SystemRoleIds.OfficeUser));

                if (!addUser.Succeeded)
                    return FailureFromIdentity(addUser);
            }
        }

        // 2️⃣ Promote target user
        if (!await userManager.IsInRoleAsync(targetUser, nameof(SystemRoleIds.OfficeAdmin)))
        {
            var promote = await userManager.AddToRoleAsync(
                targetUser,
                nameof(SystemRoleIds.OfficeAdmin));

            if (!promote.Succeeded)
                return FailureFromIdentity(promote);
        }

        return Result.Ok(Unit.Value);
    }

    private static Result<Unit> FailureFromIdentity(IdentityResult res)
        => Result.Fail<Unit>(string.Join(", ", res.Errors.Select(e => e.Description)));
}
