using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class TestAttemptPartConfiguration : BaseEntityConfiguration<TestAttemptPart>
{
    public override void Configure(EntityTypeBuilder<TestAttemptPart> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new
            {
                x.TestAttemptId,
                x.ExamPartId
            })
            .IsUnique();

        builder.HasOne(x => x.TestAttempt)
            .WithMany(x => x.Parts)
            .HasForeignKey(x => x.TestAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExamPart)
            .WithMany()
            .HasForeignKey(x => x.ExamPartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

