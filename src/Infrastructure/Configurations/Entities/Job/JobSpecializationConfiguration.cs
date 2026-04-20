using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities.Job;

public class JobSpecializationConfiguration : IEntityTypeConfiguration<JobSpecialization>
{
    public void Configure(EntityTypeBuilder<JobSpecialization> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Major)
            .WithMany()
            .HasForeignKey(x => x.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubMajor)
            .WithMany()
            .HasForeignKey(x => x.SubMajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.JobId, x.MajorId, x.SubMajorId })
            .IsUnique()
            .HasFilter("[SubMajorId] IS NOT NULL AND [IsDeleted] = 0");

        builder.HasIndex(x => new { x.JobId, x.MajorId })
            .IsUnique()
            .HasFilter("[SubMajorId] IS NULL AND [IsDeleted] = 0");
    }
}
