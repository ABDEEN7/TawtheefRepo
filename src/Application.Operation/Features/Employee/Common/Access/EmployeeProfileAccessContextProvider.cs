using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Common.Access;

internal sealed class EmployeeProfileAccessContextProvider(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<Result<EmployeeProfileAccessContext>> GetAsync(CancellationToken cancellationToken)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        if (!Guid.TryParse(currentUserService.UserId, out var userId) ||
            principal?.Identity?.IsAuthenticated != true)
            return Result.Fail("Unauthorized");

        var currentUser = await userManager.Users
            .AsNoTracking()
            .Include(user => (user as OfficeUser)!.Office)
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
        if (currentUser is null) return Result.Fail("Unauthorized");

        return Result.Ok(new EmployeeProfileAccessContext(
            userId,
            currentUser,
            HasPermission(PermissionKeys.ProfileDistribution.View),
            HasPermission(PermissionKeys.ProfileApproval.View) ||
            HasPermission(PermissionKeys.ProfileApproval.Review)));

        bool HasPermission(string permission) =>
            principal.HasClaim(RoleClaimTypes.Permission, permission);
    }
}
