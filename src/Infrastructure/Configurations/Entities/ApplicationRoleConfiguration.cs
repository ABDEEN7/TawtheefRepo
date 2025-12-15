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
        builder.Property(r => r.NameAr).HasMaxLength(50);
        builder.Property(r => r.NameEn).HasMaxLength(50);
        builder.Property(r => r.DescriptionAr).HasMaxLength(150);
        builder.Property(r => r.DescriptionEn).HasMaxLength(150);

        builder.Property(r => r.IsSystemRole)
            .HasDefaultValue(false)
            .IsRequired();

        // -----------------------
        // Seed System Roles
        // -----------------------
        builder.HasData(
            new ApplicationRole
            {
                Id = Guid.Parse("1361d691-53c5-4a84-aea1-64ff134cf082"),
                Name = "SystemAdmin",
                NormalizedName = "SYSTEMADMIN",
                NameAr = "مدير النظام",
                NameEn = "System Admin",
                DescriptionAr = "مدير النظام الكامل",
                DescriptionEn = "Full system administrator",
                IsSystemRole = true
            },
            new ApplicationRole
            {
                Id = Guid.Parse("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                Name = "HRAdmin",
                NormalizedName = "HRADMIN",
                NameAr = "مدير الموارد البشرية",
                NameEn = "HR Admin",
                DescriptionAr = "مدير شؤون الموظفين",
                DescriptionEn = "Human resources administrator",
                IsSystemRole = true
            },
            new ApplicationRole
            {
                Id = Guid.Parse("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                Name = "OfficeAdmin",
                NormalizedName = "OFFICEADMIN",
                NameAr = "مدير المكتب",
                NameEn = "Office Admin",
                DescriptionAr = "مدير المكتب والصلاحيات المرتبطة",
                DescriptionEn = "Office administrator",
                IsSystemRole = true
            }
        );
    }
}
