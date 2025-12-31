using Microsoft.EntityFrameworkCore;
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
        builder.Property(c => c.IsActive).HasDefaultValue(true);
    }
}
