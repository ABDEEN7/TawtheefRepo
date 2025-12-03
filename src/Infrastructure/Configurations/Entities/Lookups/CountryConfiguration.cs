using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class CountryConfiguration : LookupBaseConfiguration<Country>
{
    public override void Configure(EntityTypeBuilder<Country> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.ISOCode).HasMaxLength(5).IsRequired();
        builder.Property(c => c.CodeAlpha).HasMaxLength(5).IsRequired();

        builder.HasData(
            new Country
            {
                Id = CountryIds.Qatar,
                BackendName = "Qatar",
                NameEn = "Qatar",
                NameAr = "قطر",
                DescriptionEn = "State of Qatar",
                DescriptionAr = "دولة قطر",
                Code = 974,
                ISOCode = "QA",
                CodeAlpha = "QAT",
                DisplayOrder = 1
            },
            new Country
            {
                Id = CountryIds.Egypt,
                BackendName = "Egypt",
                NameEn = "Egypt",
                NameAr = "مصر",
                DescriptionEn = "Arab Republic of Egypt",
                DescriptionAr = "جمهورية مصر العربية",
                Code = 20,
                ISOCode = "EG",
                CodeAlpha = "EGY",
                DisplayOrder = 2
            }
        );
    }
}
