using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Kawader;

namespace Tawtheef.Infrastructure.Configurations.Entities.Kawader;

public class KawaderQidConfiguration : IEntityTypeConfiguration<KawaderQid>
{
    public void Configure(EntityTypeBuilder<KawaderQid> builder)
    {
        builder.ToTable("KawaderQids");

        builder.Property(x => x.Qid)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.Qid).IsUnique();
    }
}
