using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.ToTable(nameof(Office), Schemas.Lookup);

        // -----------------------------
        // Office → OfficeAdmin (1 : 1)
        // -----------------------------
        builder
            .HasOne(o => o.OfficeAdmin)
            .WithMany()
            .HasForeignKey(o => o.OfficeAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------------------
        // Office → OfficeUsers (1 : many)
        // -----------------------------
        builder
            .HasMany(o => o.OfficeUsers)
            .WithOne(u => u.Office)
            .HasForeignKey(u => u.OfficeId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------
        // Properties
        // -----------------------------
        builder
            .Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .HasIndex(o => o.Code)
            .IsUnique();
    }
}
