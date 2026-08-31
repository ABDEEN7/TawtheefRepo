using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ExamCategoryTypeConfiguration : LookupBaseConfiguration<ExamCategoryType>
{
    public override void Configure(EntityTypeBuilder<ExamCategoryType> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ExamCategoryType
            {
                Id = ExamCategoryTypeIds.Specialized,
                BackendName = nameof(ExamCategoryTypeIds.Specialized),
                NameEn = "Specialized",
                NameAr = "تخصصي",
                DisplayOrder = 1
            },
            new ExamCategoryType
            {
                Id = ExamCategoryTypeIds.Educational,
                BackendName = nameof(ExamCategoryTypeIds.Educational),
                NameEn = "Educational",
                NameAr = "تربوي",
                DisplayOrder = 2
            },
            new ExamCategoryType
            {
                Id = ExamCategoryTypeIds.Skills,
                BackendName = nameof(ExamCategoryTypeIds.Skills),
                NameEn = "Skills",
                NameAr = "مهارات",
                DisplayOrder = 3
            }
        );
    }
}

