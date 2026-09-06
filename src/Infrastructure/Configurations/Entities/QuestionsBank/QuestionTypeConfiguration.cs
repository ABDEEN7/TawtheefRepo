using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionTypeConfiguration : LookupBaseConfiguration<QuestionType>
{
    public override void Configure(EntityTypeBuilder<QuestionType> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new QuestionType
            {
                Id = QuestionTypeIds.MULTIPLE_CHOICE,
                BackendName = "MULTIPLE_CHOICE",
                NameEn = "Multiple Choice",
                NameAr = "اختيار من متعدد",
                DisplayOrder = 1
            },
            new QuestionType
            {
                Id = QuestionTypeIds.TRUE_FALSE,
                BackendName = "TRUE_FALSE",
                NameEn = "True / False",
                NameAr = "صح / خطأ",
                DisplayOrder = 2
            }
        );
    }
}
