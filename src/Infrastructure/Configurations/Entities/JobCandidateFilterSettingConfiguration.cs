using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobCandidateFilterSettingConfiguration : IEntityTypeConfiguration<JobCandidateFilterSetting>
{
    public void Configure(EntityTypeBuilder<JobCandidateFilterSetting> builder)
    {
        builder.HasIndex(x => x.JobId).IsUnique();

        builder.HasMany(x => x.CandidateTypePercentages)
            .WithOne(x => x.JobCandidateFilterSetting)
            .HasForeignKey(x => x.JobCandidateFilterSettingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.NationalityPercentages)
            .WithOne(x => x.JobCandidateFilterSetting)
            .HasForeignKey(x => x.JobCandidateFilterSettingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
