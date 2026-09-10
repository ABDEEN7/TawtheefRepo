using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewCommitteeConfiguration : BaseEntityConfiguration<InterviewCommittee>
{
    public override void Configure(EntityTypeBuilder<InterviewCommittee> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEn).HasMaxLength(200);

        builder.HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InterviewTemplate)
            .WithMany(t => t.Committees)
            .HasForeignKey(x => x.InterviewTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        // One CURRENT committee per job; replacing one deactivates the previous instead of a bare unique JobId.
        builder.HasIndex(x => x.JobId)
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("UX_InterviewCommittee_ActiveJob");
    }
}
