using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class ResidentBreakdownConfiguration : IEntityTypeConfiguration<ResidentBreakdown>
{
    public void Configure(EntityTypeBuilder<ResidentBreakdown> builder)
    {
        
        builder.Property(rb => rb.Percentage)
            .HasPrecision(18, 2);
            
        builder.HasOne(rb => rb.Nationality)
            .WithMany()
            .HasForeignKey(rb => rb.NationalityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
