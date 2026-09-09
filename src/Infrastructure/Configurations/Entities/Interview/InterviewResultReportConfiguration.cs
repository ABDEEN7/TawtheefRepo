using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewResultReportConfiguration : BaseEntityConfiguration<InterviewResultReport>
{
    public override void Configure(EntityTypeBuilder<InterviewResultReport> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewSchedule)
            .WithMany()
            .HasForeignKey(x => x.InterviewScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApprovedBy)
            .WithMany()
            .HasForeignKey(x => x.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        // One report per schedule.
        builder.HasIndex(x => x.InterviewScheduleId)
            .IsUnique()
            .HasDatabaseName("UQ_ResultReport");
    }
}
