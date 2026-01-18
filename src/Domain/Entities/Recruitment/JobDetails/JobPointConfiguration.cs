using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public static class JobPointConfigurationIds
{
    public static Guid Default = Guid.Parse("4b39e8ae-991a-4a59-b0a6-0fc17a59faf8");
}

[Table(nameof(JobPointConfiguration), Schema = Schemas.Hr)]
public class JobPointConfiguration : EventEntity
{
    [Required]
    public int ApplicantCategoryMaxPoints { get; set; }

    [Required]
    public int EducationMaxPoints { get; set; }

    [Required]
    public int ExperienceMaxPoints { get; set; }

    [Required]
    public int TrainingMaxPoints { get; set; }

    [Required]
    public int CertificatesMaxPoints { get; set; }

    [Required]
    public int SkillsMaxPoints { get; set; }

    [Required]
    public int LanguagesMaxPoints { get; set; }

    [Required]
    public int MaxPoints { get; set; }

    public bool IsValid()
    {
        int total = ApplicantCategoryMaxPoints +
                    EducationMaxPoints +
                    ExperienceMaxPoints +
                    TrainingMaxPoints +
                    CertificatesMaxPoints +
                    SkillsMaxPoints +
                    LanguagesMaxPoints;

        return total == MaxPoints;
    }
}
