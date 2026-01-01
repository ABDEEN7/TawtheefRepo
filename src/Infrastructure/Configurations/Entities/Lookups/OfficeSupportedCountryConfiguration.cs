using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class OfficeSupportedCountryConfiguration : OfficeSupportedCountry
{
    public void Configure(EntityTypeBuilder<OfficeSupportedCountry> builder)
    {
        builder.HasKey(x => new { x.OfficeId, x.CountryId });

        builder.HasOne(x => x.Office)
            .WithMany(o => o.SupportedCountries)
            .HasForeignKey(x => x.OfficeId);

        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey(x => x.CountryId);
    }
}
