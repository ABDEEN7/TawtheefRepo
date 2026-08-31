using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class TestAttemptInterruptionConfiguration : BaseEntityConfiguration<TestAttemptInterruption>
{
    public override void Configure(EntityTypeBuilder<TestAttemptInterruption> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.TestAttempt)
            .WithMany()
            .HasForeignKey(x => x.TestAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestAttemptPart)
            .WithMany()
            .HasForeignKey(x => x.TestAttemptPartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResolutionAction)
            .WithMany()
            .HasForeignKey(x => x.ResolutionActionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
