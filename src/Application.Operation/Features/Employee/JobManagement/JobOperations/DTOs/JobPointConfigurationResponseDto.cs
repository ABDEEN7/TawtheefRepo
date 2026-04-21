namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobPointConfigurationResponseDto
{
    public Guid Id { get; set; }
    public int ApplicantCategoryMaxPoints { get; set; }
    public int EducationMaxPoints { get; set; }
    public int ExperienceMaxPoints { get; set; }
    public int TrainingMaxPoints { get; set; }
    public int CertificatesMaxPoints { get; set; }
    public int SkillsMaxPoints { get; set; }
    public int LanguagesMaxPoints { get; set; }
    public int MaxPoints { get; set; }
}
