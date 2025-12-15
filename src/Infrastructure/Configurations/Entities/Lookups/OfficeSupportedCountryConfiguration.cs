using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class OfficeSupportedCountryConfiguration : LookupBaseConfiguration<OfficeSupportedCountry>
{
    public override void Configure(EntityTypeBuilder<OfficeSupportedCountry> builder)
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
