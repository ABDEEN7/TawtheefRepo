using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class ResidentBreakdownConfiguration : IEntityTypeConfiguration<ResidentBreakdown>
{
    public void Configure(EntityTypeBuilder<ResidentBreakdown> builder)
    {
        builder.ToTable(nameof(ResidentBreakdown), Schemas.Hr); // ÅÖÇÝÉ åÐÇ ÇáÓØÑ

        builder.Property(rb => rb.Percentage)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(rb => rb.Nationality)
            .WithMany()
            .HasForeignKey(rb => rb.NationalityId)
            .OnDelete(DeleteBehavior.Restrict);

        // ÝåÇÑÓ
        builder.HasIndex(rb => rb.JobQuotaId);
        builder.HasIndex(rb => rb.NationalityId);
    }
}
