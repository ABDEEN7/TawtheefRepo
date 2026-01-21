using Application.Operation.Common.Interfaces.Services.Office;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Office;

public sealed class OfficeAdminProvisioner(UserManager<User> userManager)
    : IOfficeAdminProvisioner
{
    public async Task<Result<User>> CreateOfficeAdminAsync(
        string email,
        string adminNameAr,
        string adminNameEn,
        CancellationToken ct)
    {
        var normalized = email.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return Result.Fail<User>(ErrorsCodes.OfficeAdminEmailInvalid);

        // Domain factory on OfficeUser (your existing code)
        var registerResult = OfficeUser.Register(normalized, adminNameAr, adminNameEn);
        if (registerResult.IsFailed)
            return Result.Fail<User>(registerResult.Errors);

        var officeUser = registerResult.Value;

        // Important: userManager.CreateAsync has no CancellationToken overload (Identity limitation)
        var createResult = await userManager.CreateAsync(officeUser);
        if (!createResult.Succeeded)
            return Result.Fail<User>(ErrorsCodes.OfficeAdminCreationFailed);

        var roleResult = await userManager.AddToRoleAsync(officeUser, nameof(SystemRoleIds.OfficeAdmin));
        if (!roleResult.Succeeded)
            return Result.Fail<User>(ErrorsCodes.OfficeRoleAssignmentFailed);

        return Result.Ok(officeUser);
    }

    public async Task<Result> AssignOfficeAsync(Guid userId, Guid officeId, CancellationToken ct)
    {
        if (userId == Guid.Empty || officeId == Guid.Empty)
            return Result.Fail(ErrorsCodes.OfficeAdminAssignFailed);

        var user = await userManager.Users
            .OfType<OfficeUser>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return Result.Fail(ErrorsCodes.ProfileNotFound);

        user.OfficeId = officeId;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Fail(ErrorsCodes.OfficeAdminAssignFailed);

        return Result.Ok();
    }
}
