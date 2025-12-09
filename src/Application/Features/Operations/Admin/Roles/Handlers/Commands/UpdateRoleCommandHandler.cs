using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Commands;

public sealed class UpdateRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<UpdateRoleCommand, IResult<RoleDto>>
{
    public async Task<IResult<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
            return Result.Fail<RoleDto>(ErrorsCodes.RoleNotFound);

        var normalized = request.NameEn.ToUpperInvariant();
        var conflict = await roleManager.Roles.AsNoTracking()
            .AnyAsync(r => r.Id != role.Id && r.NormalizedName == normalized, cancellationToken);
        if (conflict)
            return Result.Fail<RoleDto>(ErrorsCodes.RoleNameExists);

        role.Name = request.NameEn;
        role.NormalizedName = normalized;

        var updateResult = await roleManager.UpdateAsync(role);
        if (!updateResult.Succeeded)
            return RoleClaimSync.FailureFromIdentity<RoleDto>(updateResult);

        var claims = await roleManager.GetClaimsAsync(role);

        var namesResult = await RoleClaimSync.SetNameClaimsAsync(roleManager, role, request.NameAr, request.NameEn, claims);
        if (namesResult.IsFailed)
            return Result.Fail<RoleDto>(namesResult.Errors);

        var permissions = RolePermissionHelper.Validate(request.Permissions);
        if (permissions.IsFailed)
            return Result.Fail<RoleDto>(permissions.Errors);

        var currentPermissions = claims
            .Where(c => c.Type == RoleClaimTypes.Permission)
            .Select(c => c.Value);

        var permissionResult = await RoleClaimSync.SyncPermissionsAsync(roleManager, role, currentPermissions, permissions.Value);
        if (permissionResult.IsFailed)
            return Result.Fail<RoleDto>(permissionResult.Errors);

        var updatedClaims = await roleManager.GetClaimsAsync(role);
        return Result.Ok(RoleMapping.ToDto(role, updatedClaims));
    }
}
