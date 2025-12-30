using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SkillConfiguration : LookupBaseConfiguration<Skill>
{
    public override void Configure(EntityTypeBuilder<Skill> builder)
    {
        base.Configure(builder);
        builder.HasOne(s => s.SkillType)
            .WithMany()
            .HasForeignKey(s => s.SkillTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.SkillTypeId);
    }
}
