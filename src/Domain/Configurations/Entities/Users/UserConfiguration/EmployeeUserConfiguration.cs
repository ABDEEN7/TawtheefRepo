using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Configurations.Entities.Users.UserConfiguration;

public class EmployeeUserConfiguration : ApplicationUserConfiguration<EmployeeUser>
{
    public override void Configure(EntityTypeBuilder<EmployeeUser> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new EmployeeUser
            {
                Id = new Guid("2464b38a-5439-421d-ac39-e0bfbdcbc17e"),
                AccessFailedCount = 0,
                ConcurrencyStamp = "ae0628d3-8171-4a35-b168-fee762654e7a",
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                Email = "qa.e@tawtheef.com",
                EmailConfirmed = true,
                FirstName = "T.",
                IsDeleted = false,
                LastName = "QA",
                LockoutEnabled = false,
                NormalizedEmail = "QA.E@TAWTHEEF.COM",
                NormalizedUserName = "QA.E@TAWTHEEF.COM",
                OtpAttempts = 0,
                //Admin@123
                PasswordHash = "AQAAAAIAAYagAAAAEGcM+djZ3c2Q/N1kjZpDwcwjH2sfsRHkNS5H4lObrCaoiN238MHwgDFbLXiNsm5J4A==",
                PhoneNumberConfirmed = false,
                SecurityStamp = "55669db7-ef3a-493f-89a6-a1d608403f83",
                TwoFactorEnabled = false,
                UserName = "qa.e@tawtheef.com",
                UserTypeId = UserTypeIds.Employee
            }
        );
    }
}
