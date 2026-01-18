using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities.Job;

public class JobPointConfigurationConfiguration : IEntityTypeConfiguration<JobPointConfiguration>
{
    public void Configure(EntityTypeBuilder<JobPointConfiguration> builder)
    {
        builder.HasData(
            new JobPointConfiguration
            {
                Id = JobPointConfigurationIds.Default,
                ApplicantCategoryMaxPoints = 0,
                EducationMaxPoints = 0,
                ExperienceMaxPoints = 0,
                TrainingMaxPoints = 0,
                CertificatesMaxPoints = 0,
                SkillsMaxPoints = 0,
                LanguagesMaxPoints = 0,
                MaxPoints = 0,
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                IsDeleted = false,
            }
        );
    }
}
