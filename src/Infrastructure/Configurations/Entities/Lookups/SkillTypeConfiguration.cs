using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SkillTypeConfiguration : LookupBaseConfiguration<SkillType>
{
    public override void Configure(EntityTypeBuilder<SkillType> builder)
    {
        base.Configure(builder);
        builder.HasData(
           new SkillType
           {
               Id = SkillTypeIds.Educational,
               BackendName = nameof(SkillTypeIds.Educational),
               NameEn = "Educational",
               NameAr = " ⁄·Ì„Ì",
               DescriptionAr = "„Â«—«   ⁄·Ì„Ì…",
               DescriptionEn = "Educational skills",
               DisplayOrder = 1
           },
    new SkillType
    {
        Id = SkillTypeIds.Technical,
        BackendName = nameof(SkillTypeIds.Technical),
        NameEn = "Technical",
        NameAr = " ﬁ‰Ì",
        DescriptionAr = "„Â«—«   ﬁ‰Ì…",
        DescriptionEn = "Technical skills",
        DisplayOrder = 2
    },
    new SkillType
    {
        Id = SkillTypeIds.Professional,
        BackendName = nameof(SkillTypeIds.Professional),
        NameEn = "Professional",
        NameAr = "„Â‰Ì",
        DescriptionAr = "„Â«—«  „Â‰Ì…",
        DescriptionEn = "Professional skills",
        DisplayOrder = 3
    },
    new SkillType
    {
        Id = SkillTypeIds.Other,
        BackendName = nameof(SkillTypeIds.Other),
        NameEn = "Other",
        NameAr = "√Œ—Ï",
        DescriptionAr = "„Â«—«  √Œ—Ï „ ‰Ê⁄…",
        DescriptionEn = "Other miscellaneous skills",
        DisplayOrder = 6
    }

        );
    }
}
