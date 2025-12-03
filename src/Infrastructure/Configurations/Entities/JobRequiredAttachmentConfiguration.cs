using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobRequiredAttachmentConfiguration : IEntityTypeConfiguration<JobRequiredAttachment>
{
    public void Configure(EntityTypeBuilder<JobRequiredAttachment> builder)
    {
        builder.ToTable(nameof(JobRequiredAttachment), Schemas.Hr);

        builder.Property(a => a.Title).HasMaxLength(200).IsRequired();
        builder.Property(a => a.IsMandatory).IsRequired();

        // فهارس
        builder.HasIndex(a => a.JobId);
    }
}
