using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionBankRequestTypeConfiguration : LookupBaseConfiguration<QuestionBankRequestType>
{
    public override void Configure(EntityTypeBuilder<QuestionBankRequestType> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new QuestionBankRequestType
            {
                Id = QuestionBankRequestTypeIds.CREATE,
                BackendName = "CREATE",
                NameEn = "Create",
                NameAr = "إنشاء",
                DisplayOrder = 1
            },
            new QuestionBankRequestType
            {
                Id = QuestionBankRequestTypeIds.MAINTENANCE,
                BackendName = "MAINTENANCE",
                NameEn = "Maintenance",
                NameAr = "صيانة",
                DisplayOrder = 2
            }
        );
    }
}
