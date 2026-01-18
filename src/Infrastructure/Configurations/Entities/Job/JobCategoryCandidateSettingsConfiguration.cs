using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities.Job;

public class JobCategoryCandidateSettingsConfiguration : IEntityTypeConfiguration<JobCategoryCandidateSettings>
{
    public void Configure(EntityTypeBuilder<JobCategoryCandidateSettings> builder)
    {
        builder.HasData(
            new JobCategoryCandidateSettings
            {
                Id = JobCategoryCandidateSettingsIds.Default,
                AcademicJobVacancies = 1,
                LaborJobVacancies = 1,
                AdministrativeJobVacancies = 1,
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                IsDeleted = false,
            }
        );
    }
}
