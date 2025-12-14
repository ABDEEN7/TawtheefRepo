using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Persistence.Configurations.Recruitment.JobDetails;

public class JobTabReviewNoteConfiguration : IEntityTypeConfiguration<JobTabReviewNote>
{
    public void Configure(EntityTypeBuilder<JobTabReviewNote> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Tab)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.TabStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(x => x.Job)
            .WithMany(j => j.TabReviewNotes)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Attachments)
             .WithOne(x => x.JobTabReviewNote)
             .HasForeignKey(x => x.JobTabReviewNoteId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}
