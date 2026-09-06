using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewTemplateVersionConfiguration : BaseEntityConfiguration<InterviewTemplateVersion>
{
    public override void Configure(EntityTypeBuilder<InterviewTemplateVersion> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewTemplate)
            .WithMany()
            .HasForeignKey(x => x.InterviewTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.InterviewTemplateId, x.VersionNo }).IsUnique();

        // TemplateVersionStatus.Approved = 4. Keep this in sync if the enum ever changes.
        builder.HasIndex(x => x.InterviewTemplateId)
            .IsUnique()
            .HasFilter("[Status] = 4 AND [IsDeleted] = 0")
            .HasDatabaseName("UX_TemplateVersion_Approved");
    }
}
