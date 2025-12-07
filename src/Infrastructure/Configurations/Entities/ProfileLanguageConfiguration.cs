using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class ProfileLanguageConfiguration : IEntityTypeConfiguration<ProfileLanguage>
{
    public virtual void Configure(EntityTypeBuilder<ProfileLanguage> builder)
    {
        builder.HasOne(pl => pl.SpeakingLevel)
            .WithMany()
            .HasForeignKey(pl => pl.SpeakingLevelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pl => pl.WritingLevel)
            .WithMany()
            .HasForeignKey(pl => pl.WritingLevelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pl => pl.ReadingLevel)
            .WithMany()
            .HasForeignKey(pl => pl.ReadingLevelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
