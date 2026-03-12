using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Users.UserConfiguration;

public class EmployeeUserConfiguration : ApplicationUserConfiguration<EmployeeUser>
{
    public override void Configure(EntityTypeBuilder<EmployeeUser> builder)
    {
        base.Configure(builder);
        
        builder.HasData(
            GenerateEmployeeSuperAdmin(EmployeeSuperAdminIds.EmployeeId1, "t-m.abdin@edu.gov.qa"),
            GenerateEmployeeSuperAdmin(EmployeeSuperAdminIds.EmployeeId2, "t-a.jaber@edu.gov.qa"),
            GenerateEmployeeSuperAdmin(EmployeeSuperAdminIds.EmployeeId3, "m.alhaddad@edu.gov.qa"),
            GenerateEmployeeSuperAdmin(EmployeeSuperAdminIds.EmployeeId4, "na.almarri@edu.gov.qa")
        );
        return;

        EmployeeUser GenerateEmployeeSuperAdmin(Guid adminId,string email)
        {
            return new EmployeeUser
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
                UserTypeId = UserTypeIds.Employee
            };
        }
    }
}
