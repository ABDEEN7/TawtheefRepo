using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class DepartmentConfiguration : LookupBaseConfiguration<Department>
{
    public override void Configure(EntityTypeBuilder<Department> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new Department
            {
                Id = DepartmentIds.InformationSystems,
                ManagementId = ManagmentIds.Minister,
                BackendName = nameof(DepartmentIds.InformationSystems),
                NameEn = "Information Systems",
                NameAr = "نظام الاعتماد",
                DescriptionAr = "نظام الاعتماد",
                DescriptionEn = "Information Systems Department",
                DisplayOrder = 1
            },
            new Department
            {
                Id = DepartmentIds.HumanResources,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.HumanResources),
                NameEn = "Human Resources",
                NameAr = "الموارد البشرية",
                DescriptionAr = "الموارد البشرية",
                DescriptionEn = "Human Resources Department",
                DisplayOrder = 2
            },
            new Department
            {
                Id = DepartmentIds.AdministrativeFinancialAffairs,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.AdministrativeFinancialAffairs),
                NameEn = "Administrative and Financial Affairs",
                NameAr = "الشؤون الادارية والمالية",
                DescriptionAr = "الشؤون الادارية والمالية",
                DescriptionEn = "Administrative and Financial Affairs Department",
                DisplayOrder = 3
            },
            new Department
            {
                Id = DepartmentIds.Evaluation,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.Evaluation),
                NameEn = "Evaluation",
                NameAr = "التقويم",
                DescriptionAr = "التقويم",
                DescriptionEn = "Evaluation Department",
                DisplayOrder = 4
            },
            new Department
            {
                Id = DepartmentIds.Curriculum,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.Curriculum),
                NameEn = "Curriculum",
                NameAr = "المناهج",
                DescriptionAr = "المناهج",
                DescriptionEn = "Curriculum Department",
                DisplayOrder = 5
            },
            new Department
            {
                Id = DepartmentIds.EarlyChildhoodEducation,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.EarlyChildhoodEducation),
                NameEn = "Early Childhood Education",
                NameAr = "تعليم الطفولة المبكرة",
                DescriptionAr = "تعليم الطفولة المبكرة",
                DescriptionEn = "Early Childhood Education Department",
                DisplayOrder = 6
            },
            new Department
            {
                Id = DepartmentIds.HigherEducation,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.HigherEducation),
                NameEn = "Higher Education",
                NameAr = "التعليم العالي",
                DescriptionAr = "التعليم العالي",
                DescriptionEn = "Higher Education Department",
                DisplayOrder = 7
            },
            new Department
            {
                Id = DepartmentIds.PrimaryEducation,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.PrimaryEducation),
                NameEn = "Primary Education",
                NameAr = "التعليم الابتدائي",
                DescriptionAr = "التعليم الابتدائي",
                DescriptionEn = "Primary Education Department",
                DisplayOrder = 8
            },
            new Department
            {
                Id = DepartmentIds.SchoolAffairs,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.SchoolAffairs),
                NameEn = "School Affairs",
                NameAr = "شؤون المدارس",
                DescriptionAr = "شؤون المدارس",
                DescriptionEn = "School Affairs Department",
                DisplayOrder = 9
            },
            new Department
            {
                Id = DepartmentIds.CommunicationsMedia,
                ManagementId = ManagmentIds.TrainingCenter,
                BackendName = nameof(DepartmentIds.CommunicationsMedia),
                NameEn = "Communications and Media",
                NameAr = "الاتصالات والاعلام",
                DescriptionAr = "الاتصالات والاعلام",
                DescriptionEn = "Communications and Media Department",
                DisplayOrder = 10
            }
        );

        builder.HasOne(d => d.Management)
            .WithMany()
            .HasForeignKey(d => d.ManagementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
