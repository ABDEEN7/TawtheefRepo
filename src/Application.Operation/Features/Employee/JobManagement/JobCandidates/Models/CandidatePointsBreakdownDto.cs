namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public class CandidatePointsBreakdownDto
{
    public int CategoryPoints { get; set; }
    public int EducationPoints { get; set; }
    public int ExperiencePoints { get; set; }
    public int TrainingPoints { get; set; }
    public int SkillPoints { get; set; }
    public int LanguagePoints { get; set; }
    public int CertificatePoints { get; set; }
    public int TotalPoints { get; set; }
    public List<PointDetailDto> Details { get; set; } = new();
}

