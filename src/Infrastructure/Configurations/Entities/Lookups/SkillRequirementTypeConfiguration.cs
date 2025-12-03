using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SkillRequirementTypeConfiguration : LookupBaseConfiguration<SkillRequirementType>
{
    public override void Configure(EntityTypeBuilder<SkillRequirementType> builder)
    {
        base.Configure(builder);
        builder.HasData(
           new SkillRequirementType
           {
               Id = SkillRequirementTypeIds.Essential,
               BackendName = nameof(SkillRequirementTypeIds.Essential),
               NameEn = "Essential",
               NameAr = "أساسي",
               DescriptionEn = "Essential skill requirement",
               DescriptionAr = "متطلب مهارة أساسي",
               DisplayOrder = 1
           },
           new SkillRequirementType
           {
               Id = SkillRequirementTypeIds.Optional,
               BackendName = nameof(SkillRequirementTypeIds.Optional),
               NameEn = "Optional",
               NameAr = "اختياري",
               DescriptionEn = "Optional skill requirement",
               DescriptionAr = "متطلب مهارة اختياري",
               DisplayOrder = 2
           }
       );

    }
}
