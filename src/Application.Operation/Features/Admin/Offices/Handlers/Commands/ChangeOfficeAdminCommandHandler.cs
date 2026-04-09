using Application.Operation.Features.Admin.Offices.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Offices.Handlers.Commands;

public sealed class ChangeOfficeAdminCommandHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeOfficeAdminCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        ChangeOfficeAdminCommand request,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var targetUser = await GetOfficeUserAsync(request.UserId, request.OfficeId, ct);

            if (targetUser is null)
                return Result.Fail<Unit>(ErrorsCodes.OfficeUserNotFound);

            var officeRepo = unitOfWork.GetEntityRepository<Office>();
            var office = await officeRepo.DbSet
                .FirstOrDefaultAsync(o => o.Id == request.OfficeId, ct);

            if (office is null)
                return Result.Fail<Unit>(ErrorsCodes.OfficeNotFound);

            var currentAdmins = await GetOfficeAdminsAsync(request.OfficeId);
            foreach (var admin in currentAdmins.Where(a => a.Id != targetUser.Id))
            {
                var demoteResult = await DemoteAdminAsync(admin);
                if (demoteResult.IsFailed)
                    return demoteResult;
            }

            var promoteResult = await PromoteToAdminAsync(targetUser);
            if (promoteResult.IsFailed)
                return promoteResult;

            office.OfficeAdminId = targetUser.Id;
            await unitOfWork.SaveChangesAsync(ct);

            return Result.Ok(Unit.Value);
        }, cancellationToken);
    }


    private async Task<OfficeUser?> GetOfficeUserAsync(Guid userId, Guid officeId, CancellationToken ct)
    {
        return await userManager.Users.OfType<OfficeUser>()
            .FirstOrDefaultAsync(u => u.Id == userId && u.OfficeId == officeId, ct);
    }

    private async Task<List<OfficeUser>> GetOfficeAdminsAsync(Guid officeId)
    {
        var admins = await userManager
            .GetUsersInRoleAsync(nameof(SystemRoleIds.OfficeAdmin));

        return admins.OfType<OfficeUser>()
            .Where(u => u.OfficeId == officeId).ToList();
    }

    /// <summary>
    /// remove role from old admin and add office user role to he
    /// </summary>
    /// <param name="admin"></param>
    /// <returns></returns>
    private async Task<Result<Unit>> DemoteAdminAsync(OfficeUser admin)
    {
        var removeAdmin = await userManager.RemoveFromRoleAsync(admin,
            nameof(SystemRoleIds.OfficeAdmin));

        if (!removeAdmin.Succeeded)
            return IdentityFailure(removeAdmin);

        if (!await userManager.IsInRoleAsync(admin, nameof(SystemRoleIds.OfficeUser)))
        {
            var addUserRole = await userManager.AddToRoleAsync(admin,
                nameof(SystemRoleIds.OfficeUser));

            if (!addUserRole.Succeeded)
                return IdentityFailure(addUserRole);
        }

        return Result.Ok(Unit.Value);
    }

    private async Task<Result<Unit>> PromoteToAdminAsync(OfficeUser user)
    {
        if (await userManager.IsInRoleAsync(user, nameof(SystemRoleIds.OfficeAdmin)))
            return Result.Ok(Unit.Value);

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Contains(nameof(SystemRoleIds.SystemAdmin), StringComparer.OrdinalIgnoreCase))
            return Result.Fail<Unit>(ErrorsCodes.SystemAdminAssignmentNotAllowed);

        var removableSystemRoles = currentRoles
            .Where(r => r.Equals(nameof(SystemRoleIds.OfficeUser), StringComparison.OrdinalIgnoreCase) ||
                        r.Equals(nameof(SystemRoleIds.Employee), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (removableSystemRoles.Length > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, removableSystemRoles);
            if (!removeResult.Succeeded)
                return IdentityFailure(removeResult);
        }

        var promote = await userManager.AddToRoleAsync(user,
            nameof(SystemRoleIds.OfficeAdmin));

        if (!promote.Succeeded)
            return IdentityFailure(promote);

        return Result.Ok(Unit.Value);
    }

    private static Result<Unit> IdentityFailure(IdentityResult result)
        => Result.Fail<Unit>(
            result.Errors.Select(e => e.Description));
}

