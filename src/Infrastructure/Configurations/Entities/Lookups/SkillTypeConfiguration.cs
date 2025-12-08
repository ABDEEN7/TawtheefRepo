using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SkillTypeConfiguration : LookupBaseConfiguration<SkillType>
{
    public override void Configure(EntityTypeBuilder<SkillType> builder)
    {
        base.Configure(builder);
    }
}
