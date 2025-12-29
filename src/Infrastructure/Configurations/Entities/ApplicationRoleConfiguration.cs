using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public sealed class ApplicationRoleConfiguration
    : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
            new ApplicationRole
            {
                Id = SystemRoleIds.SystemAdmin,
                Name = nameof(SystemRoleIds.SystemAdmin),
                NormalizedName = nameof(SystemRoleIds.SystemAdmin).ToUpper(),
                NameAr = "مدير النظام",
                NameEn = "System Admin",
                DescriptionAr = "مدير النظام الكامل",
                DescriptionEn = "Full system administrator",
                IsSystemRole = true,
                ConcurrencyStamp = "00ef28cd-ca1e-4a83-9a29-7e6e07c50e78"
            },
            new ApplicationRole
            {
                Id = SystemRoleIds.Employee,
                Name = nameof(SystemRoleIds.Employee),
                NormalizedName = nameof(SystemRoleIds.Employee).ToUpper(),
                NameAr = "موظف",
                NameEn = "Employee",
                DescriptionAr = "موظف اساسي",
                DescriptionEn = "Basic Employee",
                IsSystemRole = true,
                ConcurrencyStamp = "82bb983f-1a91-4572-8d70-0598885b0514"
            },
            new ApplicationRole
            {
                Id = SystemRoleIds.OfficeAdmin,
                Name = nameof(SystemRoleIds.OfficeAdmin),
                NormalizedName = nameof(SystemRoleIds.OfficeAdmin).ToUpper(),
                NameAr = "مدير المكتب",
                NameEn = "Office Admin",
                DescriptionAr = "مدير المكتب والصلاحيات المرتبطة",
                DescriptionEn = "Office administrator",
                IsSystemRole = true,
                ConcurrencyStamp = "51961925-7d86-4e6a-bf60-646475d80319"
            },
            new ApplicationRole
            {
                Id = SystemRoleIds.OfficeUser,
                Name = nameof(SystemRoleIds.OfficeUser),
                NormalizedName = nameof(SystemRoleIds.OfficeUser).ToUpper(),
                NameAr = "موظف المكتب",
                NameEn = "Office User",
                DescriptionAr = "موظف المكتب العادي",
                DescriptionEn = "Office user",
                IsSystemRole = true,
                ConcurrencyStamp = "d0f6fe80-b684-40c0-9351-727e2feef829"
            },
            new ApplicationRole
            {
                Id = SystemRoleIds.EmployeeSuperAdmin,
                Name = nameof(SystemRoleIds.EmployeeSuperAdmin),
                NormalizedName = nameof(SystemRoleIds.EmployeeSuperAdmin).ToUpper(),
                NameAr = "موظف بصلاحية كاملة",
                NameEn = "Employee Super Admin",
                DescriptionAr = "موظف بصلاحية كاملة",
                DescriptionEn = "Employee Super Admin",
                IsSystemRole = false,
                ConcurrencyStamp = "b8d1862f-102c-4e76-9d53-3e9452930ce0"
            }
        );
    }
}
