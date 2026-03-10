using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Security;

public class AdminRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityUserRole<Guid>
            {
                UserId = AdminUserIds.Admin1UserId,
                RoleId = SystemRoleIds.SystemAdmin,
            },
            new IdentityUserRole<Guid>
            {
                UserId = AdminUserIds.Admin2UserId,
                RoleId = SystemRoleIds.SystemAdmin,
            },
            new IdentityUserRole<Guid>
            {
                UserId = AdminUserIds.Admin3UserId,
                RoleId = SystemRoleIds.SystemAdmin,
            }
        );
    }
}
