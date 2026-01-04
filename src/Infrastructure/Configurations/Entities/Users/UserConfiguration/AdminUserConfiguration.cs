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
            GenerateAdminUser(AdminUserIds.Admin1UserId, "t-m.abdin-dev@edu.gov.qa"),
            GenerateAdminUser(AdminUserIds.Admin2UserId, "t-a.jaber-dev@edu.gov.qa"),
            GenerateAdminUser(AdminUserIds.Admin3UserId, "t-m.khatatbeh-dev@edu.gov.qa"),
            GenerateAdminUser(AdminUserIds.Admin4UserId, "t-hu.ahmed-dev@edu.gov.qa")
        );
        return;

        AdminUser GenerateAdminUser(Guid adminId,string email)
        {
            return new AdminUser
            {
                Id = adminId,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                
                AccessFailedCount = 0,
                ConcurrencyStamp = "75a677a7-c93d-4940-8666-4d648343104c",
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                EmailConfirmed = true,
                FullNameEn = email.Split('@')[0],
                FullNameAr = email.Split('@')[0],
                IsDeleted = false,
                LockoutEnabled = false,
                OtpAttempts = 0,
                PhoneNumberConfirmed = false,
                SecurityStamp = "a984b6f5-e904-44b0-8d0d-5e93c07b1510",
                TwoFactorEnabled = false,
                UserTypeId = UserTypeIds.Admin
            };
        }
    }
}

// assign admin role for user
