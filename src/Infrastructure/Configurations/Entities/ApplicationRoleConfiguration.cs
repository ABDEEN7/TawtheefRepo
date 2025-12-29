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
                IsSystemRole = true
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
                IsSystemRole = true
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
                IsSystemRole = true
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
                IsSystemRole = false
            }
        );
    }
}
