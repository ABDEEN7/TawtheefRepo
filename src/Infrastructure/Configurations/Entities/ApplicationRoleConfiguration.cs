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
                Id = ApplicationRoleIds.SystemAdmin,
                Name = "SystemAdmin",
                NormalizedName = nameof(ApplicationRoleIds.SystemAdmin),
                NameAr = "مدير النظام",
                NameEn = "System Admin",
                DescriptionAr = "مدير النظام الكامل",
                DescriptionEn = "Full system administrator",
                IsSystemRole = true
            },
            new ApplicationRole
            {
                Id = ApplicationRoleIds.HRAdmin,
                Name = "HRAdmin",
                NormalizedName = nameof(ApplicationRoleIds.HRAdmin),
                NameAr = "مدير الموارد البشرية",
                NameEn = "HR Admin",
                DescriptionAr = "مدير شؤون الموظفين",
                DescriptionEn = "Human resources administrator",
                IsSystemRole = true
            },
            new ApplicationRole
            {
                Id = ApplicationRoleIds.OfficeAdmin,
                Name = "OfficeAdmin",
                NormalizedName = nameof(ApplicationRoleIds.OfficeAdmin),
                NameAr = "مدير المكتب",
                NameEn = "Office Admin",
                DescriptionAr = "مدير المكتب والصلاحيات المرتبطة",
                DescriptionEn = "Office administrator",
                IsSystemRole = true
            },
            new ApplicationRole
            {
                Id = ApplicationRoleIds.OfficeUser,
                Name = "OfficeUser",
                NormalizedName = nameof(ApplicationRoleIds.OfficeUser),
                NameAr = "موظف المكتب",
                NameEn = "Office User",
                DescriptionAr = "موظف المكتب العادي",
                DescriptionEn = "Office user",
                IsSystemRole = true
            }
        );
    }
}
