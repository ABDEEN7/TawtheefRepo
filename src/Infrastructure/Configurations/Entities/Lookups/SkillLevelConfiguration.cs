using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SkillLevelConfiguration : LookupBaseConfiguration<SkillLevel>
{
    public override void Configure(EntityTypeBuilder<SkillLevel> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new SkillLevel
            {
                Id = SkillLevelIds.Expert,
                BackendName = nameof(SkillLevelIds.Expert),
                NameEn = "Expert",
                NameAr = "خبير",
                DisplayOrder = 1
            },
            new SkillLevel
            {
                Id = SkillLevelIds.Advanced,
                BackendName = nameof(SkillLevelIds.Advanced),
                NameEn = "Advanced",
                NameAr = "متقدم",
                DisplayOrder = 2
            },
            new SkillLevel
            {
                Id = SkillLevelIds.Intermediate,
                BackendName = nameof(SkillLevelIds.Intermediate),
                NameEn = "Intermediate",
                NameAr = "متوسط",
                DisplayOrder = 3
            },
            new SkillLevel
            {
                Id = SkillLevelIds.Basic,
                BackendName = nameof(SkillLevelIds.Basic),
                NameEn = "Basic",
                NameAr = "أساسي",
                DisplayOrder = 4
            }
        );
    }
}
