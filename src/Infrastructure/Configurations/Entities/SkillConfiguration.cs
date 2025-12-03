using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable(nameof(Skill), Schemas.Lookup);

        builder.Property(s => s.Description).HasMaxLength(500);

        // العلاقات
        builder.HasOne(s => s.Major)
            .WithMany()
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.SkillType)
            .WithMany()
            .HasForeignKey(s => s.SkillTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.SkillRequirementType)
            .WithMany()
            .HasForeignKey(s => s.SkillRequirementTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // فهارس
        builder.HasIndex(s => s.MajorId);
        builder.HasIndex(s => s.SkillTypeId);
        builder.HasIndex(s => s.SkillRequirementTypeId);
    }
}
