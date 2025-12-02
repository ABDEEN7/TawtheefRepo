using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class MajorConfiguration : LookupBaseConfiguration<Major>
{
    public override void Configure(EntityTypeBuilder<Major> builder)
    {
        base.Configure(builder);
        builder.HasOne(m => m.Parent)
            .WithMany(sm=> sm.SubMajors)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
