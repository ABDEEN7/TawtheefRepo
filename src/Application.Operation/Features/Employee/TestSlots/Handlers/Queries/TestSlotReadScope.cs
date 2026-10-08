using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

internal readonly record struct TestSlotReadScopeState(
    Guid UserId,
    bool RestrictToAssignedSlots);

internal static class TestSlotReadScope
{
    internal static async Task<TestSlotReadScopeState> ResolveAsync(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        CancellationToken token)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId))
            return new TestSlotReadScopeState(Guid.Empty, true);

        var userRoles = unitOfWork.Context.Set<IdentityUserRole<Guid>>()
            .Where(x => x.UserId == userId);

        var hasTestSlotStaffMemberRole = await userRoles.AnyAsync(
            x => x.RoleId == SystemRoleIds.TestSlotStaffMember,
            token);

        if (!hasTestSlotStaffMemberRole)
            return new TestSlotReadScopeState(userId, false);

        var hasBroaderTestSlotAccess = await userRoles
            .Where(x => x.RoleId != SystemRoleIds.TestSlotStaffMember)
            .Join(
                unitOfWork.Context.Set<IdentityRoleClaim<Guid>>(),
                userRole => userRole.RoleId,
                claim => claim.RoleId,
                (_, claim) => claim)
            .AnyAsync(
                claim =>
                    claim.ClaimType == RoleClaimTypes.Permission &&
                    claim.ClaimValue == PermissionKeys.TestSlots.View,
                token);

        return new TestSlotReadScopeState(
            userId,
            !hasBroaderTestSlotAccess);
    }

    internal static IQueryable<TestSlot> Apply(
        IQueryable<TestSlot> query,
        IUnitOfWork unitOfWork,
        TestSlotReadScopeState scope)
    {
        if (!scope.RestrictToAssignedSlots)
            return query;

        return query.Where(slot =>
            unitOfWork.Context.Set<TestSlotStaff>().Any(staff =>
                staff.TestSlotId == slot.Id &&
                staff.StaffUserId == scope.UserId &&
                staff.IsActive));
    }

    internal static async Task<bool> CanAccessAsync(
        IUnitOfWork unitOfWork,
        TestSlotReadScopeState scope,
        Guid testSlotId,
        CancellationToken token)
    {
        if (!scope.RestrictToAssignedSlots)
            return true;

        return await unitOfWork.Context.Set<TestSlotStaff>().AnyAsync(
            staff =>
                staff.TestSlotId == testSlotId &&
                staff.StaffUserId == scope.UserId &&
                staff.IsActive,
            token);
    }
}
