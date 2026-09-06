using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionChangeTypeConfiguration : LookupBaseConfiguration<QuestionChangeType>
{
    public override void Configure(EntityTypeBuilder<QuestionChangeType> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new QuestionChangeType
            {
                Id = QuestionChangeTypeIds.ADD,
                BackendName = "ADD",
                NameEn = "Add",
                NameAr = "إضافة",
                DisplayOrder = 1
            },
            new QuestionChangeType
            {
                Id = QuestionChangeTypeIds.UPDATE,
                BackendName = "UPDATE",
                NameEn = "Update",
                NameAr = "تعديل",
                DisplayOrder = 2
            },
            new QuestionChangeType
            {
                Id = QuestionChangeTypeIds.DELETE,
                BackendName = "DELETE",
                NameEn = "Delete",
                NameAr = "حذف",
                DisplayOrder = 3
            }
        );
    }
}
