using Application.Operation.Features.Employee.Common.Access;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Security;

namespace Application.Operation.Features.Employee.Dashboard.Services.Access;

internal sealed class DashboardAccessContextProvider(
    EmployeeProfileAccessContextProvider profileAccessContextProvider,
    EmployeeJobAccessContextProvider jobAccessContextProvider,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<Result<DashboardAccessContext>> GetAsync(CancellationToken ct)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var profileAccessResult = await profileAccessContextProvider.GetAsync(ct);
        if (profileAccessResult.IsFailed)
            return Result.Fail(profileAccessResult.Errors);

        var profileAccess = profileAccessResult.Value;
        var jobAccess = jobAccessContextProvider.GetAccess();

        return Result.Ok(new DashboardAccessContext(
            profileAccess.CurrentUserId,
            profileAccess.CurrentUser,
            profileAccess.CanViewProfileDistribution,
            profileAccess.CanViewAssignedProfiles,
            HasPermission(PermissionKeys.Jobs.View) || HasPermission(PermissionKeys.Jobs.Edit),
            HasPermission(PermissionKeys.JobsInvitations.View),
            HasPermission(PermissionKeys.MinisterOffice.View),
            jobAccess.HasFullAccess));

        bool HasPermission(string permission) => principal!.HasClaim(RoleClaimTypes.Permission, permission);
    }
}
