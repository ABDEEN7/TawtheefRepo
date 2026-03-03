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
                NameAr = "تعليمي",
                DescriptionEn = "Educational skills acquired through formal education",
                DescriptionAr = "مهارات تعليمية تم اكتسابها من خلال التعليم الرسمي",
                DisplayOrder = 1
            },
            new SkillType
            {
                Id = SkillTypeIds.Technical,
                BackendName = nameof(SkillTypeIds.Technical),
                NameEn = "Technical",
                NameAr = "تقني",
                DescriptionEn = "Technical or hard skills related to specific tools, technologies, or methodologies",
                DescriptionAr = "مهارات تقنية أو عملية مرتبطة بأدوات أو تقنيات أو منهجيات محددة",
                DisplayOrder = 2
            },
            new SkillType
            {
                Id = SkillTypeIds.Professional,
                BackendName = nameof(SkillTypeIds.Professional),
                NameEn = "Professional",
                NameAr = "مهني",
                DescriptionEn = "Professional soft skills and workplace competencies",
                DescriptionAr = "مهارات مهنية وسلوكية مرتبطة ببيئة العمل",
                DisplayOrder = 3
            },
            new SkillType
            {
                Id = SkillTypeIds.Other,
                BackendName = nameof(SkillTypeIds.Other),
                NameEn = "Other",
                NameAr = "أخرى",
                DescriptionEn = "Other types of skills not categorized above",
                DescriptionAr = "أنواع أخرى من المهارات غير المصنفة أعلاه",
                DisplayOrder = 4
            }
        );
    }
}
