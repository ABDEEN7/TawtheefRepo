using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public sealed class ApplicationRoleClaimsConfiguration
    : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        foreach (var claim in PermissionNames.SystemAdminPermissions)
        {
            builder.HasData(new IdentityRoleClaim<Guid> { RoleId = SystemRoleIds.SystemAdmin, ClaimType = RoleClaimTypes.Permission , ClaimValue = claim });
        }
        foreach (var claim in PermissionNames.EmployeePermissions)
        {
            builder.HasData(new IdentityRoleClaim<Guid> { RoleId = SystemRoleIds.Employee, ClaimType = RoleClaimTypes.Permission , ClaimValue = claim });
        }
        foreach (var claim in PermissionNames.OfficeAdminPermissions)
        {
            builder.HasData(new IdentityRoleClaim<Guid> { RoleId = SystemRoleIds.OfficeAdmin, ClaimType = RoleClaimTypes.Permission , ClaimValue = claim });
        }
        foreach (var claim in PermissionNames.OfficeUserPermissions)
        {
            builder.HasData(new IdentityRoleClaim<Guid> { RoleId = SystemRoleIds.OfficeUser, ClaimType = RoleClaimTypes.Permission , ClaimValue = claim });
        }
        foreach (var claim in PermissionNames.EmployeeSuperAdminPermissions)
        {
            builder.HasData(new IdentityRoleClaim<Guid> { RoleId = SystemRoleIds.EmployeeSuperAdmin, ClaimType = RoleClaimTypes.Permission , ClaimValue = claim });
        }
    }
}
