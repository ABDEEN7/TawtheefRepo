using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobResponsibilityConfiguration : IEntityTypeConfiguration<JobResponsibility>
{
    public void Configure(EntityTypeBuilder<JobResponsibility> builder)
    {
        builder.ToTable(nameof(JobResponsibility), Schemas.Hr);

        builder.Property(r => r.Text).HasMaxLength(500).IsRequired();

        builder.HasIndex(r => r.JobId);
    }
}
