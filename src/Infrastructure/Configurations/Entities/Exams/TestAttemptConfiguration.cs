using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class TestAttemptConfiguration : BaseEntityConfiguration<TestAttempt>
{
    public override void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.TestSessionCandidate)
            .WithMany()
            .HasForeignKey(x => x.TestSessionCandidateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

