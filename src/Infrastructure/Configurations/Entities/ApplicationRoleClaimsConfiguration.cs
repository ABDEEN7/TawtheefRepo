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
        var id = -1;

        void Seed(Guid roleId, IEnumerable<string> permissions)
        {
            foreach (var permission in permissions)
            {
                builder.HasData(new IdentityRoleClaim<Guid>
                {
                    Id = id--,
                    RoleId = roleId,
                    ClaimType = RoleClaimTypes.Permission,
                    ClaimValue = permission
                });
            }
        }

        Seed(SystemRoleIds.SystemAdmin, PermissionNames.SystemAdminPermissions);
        Seed(SystemRoleIds.Employee, PermissionNames.EmployeePermissions);
        Seed(SystemRoleIds.OfficeAdmin, PermissionNames.OfficeAdminPermissions);
        Seed(SystemRoleIds.OfficeUser, PermissionNames.OfficeUserPermissions);
        Seed(SystemRoleIds.EmployeeSuperAdmin, PermissionNames.EmployeeSuperAdminPermissions);
    }
}
