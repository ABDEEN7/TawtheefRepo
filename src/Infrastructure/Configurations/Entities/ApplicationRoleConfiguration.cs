using Microsoft.AspNetCore.Identity;
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
                Name = nameof(ApplicationRoleIds.SystemAdmin),
                NormalizedName = nameof(SystemRoleIds.SystemAdmin).ToUpper(),
                NameAr = "مدير النظام",
                NameEn = "System Admin",
                DescriptionAr = "مدير النظام الكامل",
                DescriptionEn = "Full system administrator",
                IsSystemRole = true
            },
            new ApplicationRole
            {
                Id = SystemRoleIds.HRAdmin,
                Name = nameof(SystemRoleIds.HRAdmin),
                NormalizedName = nameof(SystemRoleIds.HRAdmin).ToUpper(),
                NameAr = "مدير الموارد البشرية",
                NameEn = "HR Admin",
                DescriptionAr = "مدير شؤون الموظفين",
                DescriptionEn = "Human resources administrator",
                IsSystemRole = true
            },
            new ApplicationRole
            {
                Id = SystemRoleIds.OfficeAdmin,
                Name = nameof(SystemRoleIds.HRAdmin),
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
                Name = nameof(SystemRoleIds.HRAdmin),
                NormalizedName = nameof(SystemRoleIds.OfficeUser).ToUpper(),
                NameAr = "موظف المكتب",
                NameEn = "Office User",
                DescriptionAr = "موظف المكتب العادي",
                DescriptionEn = "Office user",
                IsSystemRole = true
            }
        );
    }
}
