using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewResultReportConfiguration : BaseEntityConfiguration<InterviewResultReport>
{
    public const string ReportNumberSequence = "InterviewResultReportNumber";
    public override void Configure(EntityTypeBuilder<InterviewResultReport> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Number)
            .HasDefaultValueSql($"NEXT VALUE FOR [{Schemas.Interview}].[{ReportNumberSequence}]");

        builder.Property(x => x.Code)
            .HasMaxLength(24)
            .HasComputedColumnSql(
                "'REP-INT-' + CAST(YEAR([CreatedDate]) AS varchar(4)) + '-' + " +
                "CASE WHEN [Number] < 10000 THEN RIGHT('0000' + CAST([Number] AS varchar(10)), 4) " +
                "ELSE CAST([Number] AS varchar(10)) END",
                stored: true);

        builder.HasOne(x => x.InterviewSchedule)
            .WithMany()
            .HasForeignKey(x => x.InterviewScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApprovedBy)
            .WithMany()
            .HasForeignKey(x => x.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        // One live report per schedule. Discarded (soft-deleted) reports stay in the table for history,
        // so the uniqueness only applies to non-deleted rows - otherwise the regenerated report after a
        // reschedule could never be inserted.
        builder.HasIndex(x => x.InterviewScheduleId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("UQ_ResultReport");
    }
}
