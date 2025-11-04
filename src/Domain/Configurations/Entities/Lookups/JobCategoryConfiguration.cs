using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Lookups;

public class JobCategoryConfiguration : LookupBaseConfiguration<JobCategory>
{
    public override void Configure(EntityTypeBuilder<JobCategory> builder)
    {
        builder.HasData(
            new JobCategory
            {
                Id = JobCategoryIds.Academic,
                BackendName = nameof(JobCategoryIds.Academic),
                NameAr = "أكاديمي",
                NameEn = "Academic",
                DescriptionAr = "فئة الوظائف الأكاديمية",
                DescriptionEn = "Category for academic jobs"
            },
            new JobCategory
            {
                Id = JobCategoryIds.Administrative,
                BackendName = nameof(JobCategoryIds.Administrative),
                NameAr = "إداري",
                NameEn = "Administrative",
                DescriptionAr = "فئة الوظائف الإدارية",
                DescriptionEn = "Category for administrative jobs"
            },
            new JobCategory
            {
                Id = JobCategoryIds.Labor,
                BackendName = nameof(JobCategoryIds.Labor),
                NameAr = "عمالي",
                NameEn = "Labor",
                DescriptionAr = "فئة الوظائف العمالية",
                DescriptionEn = "Category for labor jobs"
            }
        );
        base.Configure(builder);
    }
}
