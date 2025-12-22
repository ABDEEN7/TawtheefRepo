using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobPointsMainConfiguration : IEntityTypeConfiguration<JobPointsMain>
{
    public void Configure(EntityTypeBuilder<JobPointsMain> builder)
    {
        builder.HasMany(p => p.Details)
        .WithOne(d => d.JobPointsMain)
        .HasForeignKey(d => d.JobPointsMainId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
