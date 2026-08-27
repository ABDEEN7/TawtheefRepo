using Application.Operation.Features.Employee.Common.Access;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Access;

internal sealed class DashboardAccessContextProvider(
    EmployeeProfileAccessContextProvider profileAccessContextProvider,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<Result<DashboardAccessContext>> GetAsync(CancellationToken ct)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var profileAccessResult = await profileAccessContextProvider.GetAsync(ct);
        if (profileAccessResult.IsFailed)
            return Result.Fail(profileAccessResult.Errors);

        var profileAccess = profileAccessResult.Value;
        var canManageDashboard = HasPermission(PermissionKeys.Dashboard.Manage);
        var scope = ResolveScope(profileAccess.CurrentUser, canManageDashboard);

        return Result.Ok(new DashboardAccessContext(
            profileAccess.CurrentUserId,
            profileAccess.CurrentUser,
            canManageDashboard,
            scope));

        bool HasPermission(string permission) => principal!.HasClaim(RoleClaimTypes.Permission, permission);
    }

    private static DashboardScope ResolveScope(User currentUser, bool canManageDashboard) =>
        currentUser switch
        {
            EmployeeUser when canManageDashboard => DashboardScope.Organization,
            OfficeUser { OfficeId: not null } => DashboardScope.Office,
            _ => DashboardScope.User
        };
}
