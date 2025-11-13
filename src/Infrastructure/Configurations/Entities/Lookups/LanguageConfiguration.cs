using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class LanguageConfiguration : LookupBaseConfiguration<Language>
{
    public override void Configure(EntityTypeBuilder<Language> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new Language
            {
                Id = LanguageIds.Arabic,
                BackendName = nameof(LanguageIds.Arabic),
                NameEn = "Arabic",
                NameAr = "العربية",
                DisplayOrder = 1
            },
            new Language
            {
                Id = LanguageIds.English,
                BackendName = nameof(LanguageIds.English),
                NameEn = "English",
                NameAr = "الإنجليزية",
                DisplayOrder = 2
            }
        );
    }
}
