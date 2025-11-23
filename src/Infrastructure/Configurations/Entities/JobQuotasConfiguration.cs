using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobQuotasConfiguration : IEntityTypeConfiguration<JobQuotas>
{
    public void Configure(EntityTypeBuilder<JobQuotas> builder)
    {
        
        builder.Property(q => q.QatariCitizens)
            .HasPrecision(18, 2);
        builder.Property(q => q.QatarMother)
            .HasPrecision(18, 2);
        builder.Property(q => q.NonQatariSpouse)
            .HasPrecision(18, 2);
        builder.Property(q => q.Gcc)
            .HasPrecision(18, 2);
        builder.Property(q => q.QuGrads)
            .HasPrecision(18, 2);
        builder.Property(q => q.Residents)
            .HasPrecision(18, 2);
            
        builder.HasMany(q => q.ResidentsBreakdown)
            .WithOne(rb => rb.JobQuota)
            .HasForeignKey(rb => rb.JobQuotaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
