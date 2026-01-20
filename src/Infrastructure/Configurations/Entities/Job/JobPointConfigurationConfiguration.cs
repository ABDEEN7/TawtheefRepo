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
                ApplicantCategoryMaxPoints = 250,
                EducationMaxPoints = 100,
                ExperienceMaxPoints = 100,
                TrainingMaxPoints = 100,
                CertificatesMaxPoints = 100,
                SkillsMaxPoints = 250,
                LanguagesMaxPoints = 100,
                MaxPoints = 1000,
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                IsDeleted = false,
            }
        );
    }
}
