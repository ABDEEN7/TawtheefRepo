using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Users.UserConfiguration;

public class AdminUserConfiguration : ApplicationUserConfiguration<AdminUser>
{
    public override void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new AdminUser
            {
                Id = AdminUserIds.AdminUserId,
                AccessFailedCount = 0,
                ConcurrencyStamp = "75a677a7-c93d-4940-8666-4d648343104c",
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                Email = "admin@tawtheef.com",
                EmailConfirmed = true,
                FullNameEn = "Admin",
                FullNameAr = "Admin",
                IsDeleted = false,
                LockoutEnabled = false,
                NormalizedEmail = "ADMIN@TAWTHEEF.COM",
                NormalizedUserName = "ADMIN@TAWTHEEF.COM",
                OtpAttempts = 0,
                //Admin@123
                PasswordHash = "AQAAAAIAAYagAAAAEIgyaMwrHltJioEPUc/0/KfGb2iA529lr04a/6+LmeVbtJZ1fV3px6oeRlalT1GI/Q==",
                PhoneNumberConfirmed = false,
                SecurityStamp = "a984b6f5-e904-44b0-8d0d-5e93c07b1510",
                TwoFactorEnabled = false,
                UserName = "admin@tawtheef.com",
                UserTypeId = UserTypeIds.Admin
            }
        );
    }
}
