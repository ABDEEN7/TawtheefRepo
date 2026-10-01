using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class TestSessionConfiguration : BaseEntityConfiguration<TestSession>
{
    public override void Configure(EntityTypeBuilder<TestSession> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.SessionNo).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.SessionNo).IsUnique();
        builder.Property(x => x.GenderFilter).HasConversion<string>().HasMaxLength(16).IsRequired(false);
        builder.Property(x => x.NationalityFilter).HasConversion<string>().HasMaxLength(16).IsRequired(false);

        builder.HasOne(x => x.TestSlot)
            .WithMany()
            .HasForeignKey(x => x.TestSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Exam)
            .WithMany()
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

