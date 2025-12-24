using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobPointConfiguration), Schema = Schemas.Hr)]
public class JobPointConfiguration : EventEntity
{
    [Required]
    public Guid JobId { get; set; }

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

    public bool IsValid()
    {
        int total = ApplicantCategoryMaxPoints +
                    EducationMaxPoints +
                    ExperienceMaxPoints +
                    TrainingMaxPoints +
                    CertificatesMaxPoints +
                    SkillsMaxPoints +
                    LanguagesMaxPoints;

        return total == 1000;
    }
}
