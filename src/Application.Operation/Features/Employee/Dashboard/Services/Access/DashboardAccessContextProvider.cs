using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Access;

internal sealed class DashboardAccessContextProvider(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<Result<DashboardAccessContext>> GetAsync(CancellationToken ct)
    {
        var userIdText = currentUserService.UserId;
        var principal = httpContextAccessor.HttpContext?.User;
        if (string.IsNullOrWhiteSpace(userIdText) ||
            !Guid.TryParse(userIdText, out var userId) ||
            principal?.Identity?.IsAuthenticated != true)
            return Result.Fail("Unauthorized");

        var currentUser = await userManager.Users
            .AsNoTracking()
            .Include(user => (user as OfficeUser)!.Office)
            .FirstOrDefaultAsync(user => user.Id == userId, ct);
        if (currentUser is null) return Result.Fail("Unauthorized");

        bool HasPermission(string permission) => principal.HasClaim(RoleClaimTypes.Permission, permission);

        return Result.Ok(new DashboardAccessContext(
            userId,
            currentUser,
            HasPermission(PermissionKeys.ProfileDistribution.View),
            HasPermission(PermissionKeys.ProfileApproval.View) || HasPermission(PermissionKeys.ProfileApproval.Review),
            HasPermission(PermissionKeys.Jobs.View) || HasPermission(PermissionKeys.Jobs.Edit),
            HasPermission(PermissionKeys.JobsInvitations.View),
            HasPermission(PermissionKeys.MinisterOffice.View),
            principal.HasFullJobAccess()));
    }
}
