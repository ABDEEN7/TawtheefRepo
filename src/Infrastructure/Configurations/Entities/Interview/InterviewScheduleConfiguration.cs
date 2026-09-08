using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewScheduleConfiguration : BaseEntityConfiguration<InterviewSchedule>
{
    public override void Configure(EntityTypeBuilder<InterviewSchedule> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TitleAr).IsRequired().HasMaxLength(200);
        builder.Property(x => x.TitleEn).HasMaxLength(200);

        builder.HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JobInterviewTemplate)
            .WithMany()
            .HasForeignKey(x => x.JobInterviewTemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
