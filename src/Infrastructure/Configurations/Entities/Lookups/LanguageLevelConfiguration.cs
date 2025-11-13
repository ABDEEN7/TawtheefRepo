using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class LanguageLevelConfiguration : LookupBaseConfiguration<LanguageLevel>
{
    public override void Configure(EntityTypeBuilder<LanguageLevel> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new LanguageLevel
            {
                Id = LanguageLevelIds.Basic,
                BackendName = nameof(LanguageLevelIds.Basic),
                NameEn = "Basic",
                NameAr = "أساسي",
                DisplayOrder = 1
            },
            new LanguageLevel
            {
                Id = LanguageLevelIds.Intermediate,
                BackendName = nameof(LanguageLevelIds.Intermediate),
                NameEn = "Intermediate",
                NameAr = "متوسط",
                DisplayOrder = 2
            },
            new LanguageLevel
            {
                Id = LanguageLevelIds.Advanced,
                BackendName = nameof(LanguageLevelIds.Advanced),
                NameEn = "Advanced",
                NameAr = "متقدم",
                DisplayOrder = 3
            },
            new LanguageLevel
            {
                Id = LanguageLevelIds.Native,
                BackendName = nameof(LanguageLevelIds.Native),
                NameEn = "Native",
                NameAr = "لغة أم",
                DisplayOrder = 4
            }
        );
    }
}
