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
                DescriptionEn = "Educational skills acquired through formal education",
                DescriptionAr = "„Â«—«   ⁄·Ì„Ì…  „ «·Õ’Ê· ⁄·ÌÂ« „‰ Œ·«· «· ⁄·Ì„ «·—”„Ì",
                DisplayOrder = 1
            },
            new SkillType
            {
                Id = SkillTypeIds.Technical,
                BackendName = nameof(SkillTypeIds.Technical),
                NameEn = "Technical",
                NameAr = " ﬁ‰Ì",
                DescriptionEn = "Technical or hard skills related to specific tools, technologies, or methodologies",
                DescriptionAr = "„Â«—«   ﬁ‰Ì… √Ê ’·»…   ⁄·ﬁ »√œÊ«  √Ê  ﬁ‰Ì«  √Ê „‰ÂÃÌ«  „Õœœ…",
                DisplayOrder = 2
            },
            new SkillType
            {
                Id = SkillTypeIds.Professional,
                BackendName = nameof(SkillTypeIds.Professional),
                NameEn = "Professional",
                NameAr = "„Â‰Ì",
                DescriptionEn = "Professional soft skills and workplace competencies",
                DescriptionAr = "„Â«—«  „Â‰Ì… ‰«⁄„… Êﬂ›«¡«  „ﬂ«‰ «·⁄„·",
                DisplayOrder = 3
            },
            new SkillType
            {
                Id = SkillTypeIds.Other,
                BackendName = nameof(SkillTypeIds.Other),
                NameEn = "Other",
                NameAr = "√Œ—Ï",
                DescriptionEn = "Other types of skills not categorized above",
                DescriptionAr = "√‰Ê«⁄ √Œ—Ï „‰ «·„Â«—«  €Ì— «·„’‰›… √⁄·«Â",
                DisplayOrder = 4
            }
        );
    }
}
