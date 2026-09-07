using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class JobInterviewTemplateConfiguration : BaseEntityConfiguration<JobInterviewTemplate>
{
    public override void Configure(EntityTypeBuilder<JobInterviewTemplate> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InterviewTemplateVersion)
            .WithMany()
            .HasForeignKey(x => x.InterviewTemplateVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.JobId)
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("UX_JobInterviewTemplate_Active");
    }
}
