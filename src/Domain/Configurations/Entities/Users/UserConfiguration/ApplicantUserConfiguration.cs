using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Configurations.Entities.Users.UserConfiguration;

public class ApplicantUserConfiguration : ApplicationUserConfiguration<ApplicantUser>
{
    public override void Configure(EntityTypeBuilder<ApplicantUser> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ApplicantUser
            {
                Id = new Guid("bf879671-62de-4f17-8c07-38b6e6619fe0"),
                AccessFailedCount = 0,
                ConcurrencyStamp = "af24df87-26d1-4f94-b84b-410bbbe14859",
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                Email = "qa.a@tawtheef.com",
                EmailConfirmed = true,
                GivenNameEn = "S.",
                IsDeleted = false,
                FamilyNameEn = "QA",
                LockoutEnabled = false,
                NormalizedEmail = "QA.A@TAWTHEEF.COM",
                NormalizedUserName = "QA.A@TAWTHEEF.COM",
                OtpAttempts = 0,
                //Admin@123
                PasswordHash = "AQAAAAIAAYagAAAAEN8QCL2z2kO862Y8bQpTxN7RPyssbCDnnWOBERadOw0vaVF5WAL3D/axQfbD1BgXKA==",
                PhoneNumberConfirmed = false,
                SecurityStamp = "18cc15bc-1783-40d9-a3b5-d26dd57c3f6c",
                TwoFactorEnabled = false,
                UserName = "qa.a@tawtheef.com",
                UserTypeId = UserTypeIds.Applicant
            }
        );
    }
}
