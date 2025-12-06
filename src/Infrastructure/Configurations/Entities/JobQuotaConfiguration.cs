using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobQuotaConfiguration : IEntityTypeConfiguration<JobQuota>
{
    public void Configure(EntityTypeBuilder<JobQuota> builder)
    {
        builder.HasOne(q => q.Job)
            .WithOne(j => j.JobQuota)
            .HasForeignKey<JobQuota>(q => q.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.ResidentsBreakdowns)
            .WithOne(rb => rb.JobQuota)
            .HasForeignKey(rb => rb.JobQuotaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
