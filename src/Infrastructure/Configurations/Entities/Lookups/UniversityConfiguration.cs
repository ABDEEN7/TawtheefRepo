using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class UniversityConfiguration : LookupBaseConfiguration<University>
{
    public override void Configure(EntityTypeBuilder<University> builder)
    {
        base.Configure(builder);
    }
}