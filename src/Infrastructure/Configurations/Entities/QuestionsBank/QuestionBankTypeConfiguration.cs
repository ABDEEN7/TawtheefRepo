using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionBankTypeConfiguration : LookupBaseConfiguration<QuestionBankType>
{
    public override void Configure(EntityTypeBuilder<QuestionBankType> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new QuestionBankType
            {
                Id = QuestionBankTypeIds.SPECIALIZED,
                BackendName = "SPECIALIZED",
                NameEn = "Specialized",
                NameAr = "تخصصي",
                DisplayOrder = 1
            },
            new QuestionBankType
            {
                Id = QuestionBankTypeIds.SKILLS,
                BackendName = "SKILLS",
                NameEn = "Skills",
                NameAr = "مهارات",
                DisplayOrder = 2
            },
            new QuestionBankType
            {
                Id = QuestionBankTypeIds.EDUCATIONAL,
                BackendName = "EDUCATIONAL",
                NameEn = "Educational",
                NameAr = "تربوي",
                DisplayOrder = 3
            }
        );
    }
}
