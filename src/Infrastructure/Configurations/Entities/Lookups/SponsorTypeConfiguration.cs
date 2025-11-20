using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SponsorTypeConfiguration : LookupBaseConfiguration<SponsorType>
{
    public override void Configure(EntityTypeBuilder<SponsorType> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new SponsorType
            {
                Id = SponsorTypeIds.Individual,
                BackendName = nameof(SponsorTypeIds.Individual),
                NameEn = "Individual",
                NameAr = "فرد",
                DisplayOrder = 1
            },
            new SponsorType
            {
                Id = SponsorTypeIds.Company,
                BackendName = nameof(SponsorTypeIds.Company),
                NameEn = "Company",
                NameAr = "منشأة",
                DisplayOrder = 2
            }
        );
    }
}
