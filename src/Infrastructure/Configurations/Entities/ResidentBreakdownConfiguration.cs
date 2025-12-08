using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class ResidentBreakdownConfiguration : IEntityTypeConfiguration<ResidentBreakdown>
{
    public void Configure(EntityTypeBuilder<ResidentBreakdown> builder)
    {
        builder.HasOne(rb => rb.Nationality)
            .WithMany()
            .HasForeignKey(rb => rb.NationalityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rb => rb.JobQuotaId);
        builder.HasIndex(rb => rb.NationalityId);
    }
}
