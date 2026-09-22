using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewCommitteeConfiguration : BaseEntityConfiguration<InterviewCommittee>
{
    // the sequence (COM-2026-000x) is Declared on the model in TawtheefDbContext.OnModelCreating - a sequence is model-level.
    public const string CommitteeNumberSequence = "InterviewCommitteeNumber";

    public override void Configure(EntityTypeBuilder<InterviewCommittee> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Number)
            .HasDefaultValueSql($"NEXT VALUE FOR [{Schemas.Interview}].[{CommitteeNumberSequence}]");

        // Beyond 9999 committees the number simply grows past four digits instead of wrapping.
        builder.Property(x => x.Code)
            .HasMaxLength(20)
            .HasComputedColumnSql(
                "'COM-' + CAST(YEAR([CreatedDate]) AS varchar(4)) + '-' + " +
                "CASE WHEN [Number] < 10000 THEN RIGHT('0000' + CAST([Number] AS varchar(10)), 4) " +
                "ELSE CAST([Number] AS varchar(10)) END",
                stored: true);

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
