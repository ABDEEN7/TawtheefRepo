namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobPointConfigurationRequestDto
{
    public Guid JobId { get; set; }
    public int ApplicantCategoryMaxPoints { get; set; }
    public int EducationMaxPoints { get; set; }
    public int ExperienceMaxPoints { get; set; }
    public int TrainingMaxPoints { get; set; }
    public int CertificatesMaxPoints { get; set; }
    public int SkillsMaxPoints { get; set; }
    public int LanguagesMaxPoints { get; set; }
}
