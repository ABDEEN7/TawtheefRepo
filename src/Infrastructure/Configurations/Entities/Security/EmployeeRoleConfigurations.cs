using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class EmployeeRoleConfigurations : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId1,
                RoleId = SystemRoleIds.EmployeeSuperAdmin,
            },
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId2,
                RoleId = SystemRoleIds.EmployeeSuperAdmin,
            },
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId3,
                RoleId = SystemRoleIds.EmployeeSuperAdmin,
            },
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId4,
                RoleId = SystemRoleIds.EmployeeSuperAdmin,
            },
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId1,
                RoleId = SystemRoleIds.Employee,
            },
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId2,
                RoleId = SystemRoleIds.Employee,
            },
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId3,
                RoleId = SystemRoleIds.Employee,
            },
            new IdentityUserRole<Guid>
            {
                UserId = EmployeeSuperAdminIds.EmployeeId4,
                RoleId = SystemRoleIds.Employee,
            }
        );
    }
}
