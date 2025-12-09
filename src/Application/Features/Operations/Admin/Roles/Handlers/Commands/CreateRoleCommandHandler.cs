using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Commands;

public sealed class CreateRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper)
    : IRequestHandler<CreateRoleCommand, IResult<RoleDto>>
{
    public async Task<IResult<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var normalized = request.NameEn.ToUpperInvariant();
        var exists = await roleManager.Roles.AsNoTracking()
            .AnyAsync(r => r.NormalizedName == normalized, cancellationToken);
        if (exists)
            return Result.Fail<RoleDto>(ErrorsCodes.RoleNameExists);

        var role = new IdentityRole<Guid>
        {
            Id = Guid.NewGuid(),
            Name = request.NameEn,
            NormalizedName = normalized
        };

        var createResult = await roleManager.CreateAsync(role);
        if (!createResult.Succeeded)
            return RoleClaimSync.FailureFromIdentity<RoleDto>(createResult);

        var namesResult = await RoleClaimSync.SetNameClaimsAsync(roleManager, role, request.NameAr, request.NameEn);
        if (namesResult.IsFailed)
            return Result.Fail<RoleDto>(namesResult.Errors);

        var permissions = RolePermissionHelper.Validate(request.Permissions);
        if (permissions.IsFailed)
            return Result.Fail<RoleDto>(permissions.Errors);

        var permissionResult = await RoleClaimSync.SyncPermissionsAsync(roleManager, role, Enumerable.Empty<string>(), permissions.Value);
        if (permissionResult.IsFailed)
            return Result.Fail<RoleDto>(permissionResult.Errors);

        var claims = await roleManager.GetClaimsAsync(role);
        return Result.Ok(mapper.Map<RoleDto>(new RoleWithClaims(role, claims)));
    }
}
