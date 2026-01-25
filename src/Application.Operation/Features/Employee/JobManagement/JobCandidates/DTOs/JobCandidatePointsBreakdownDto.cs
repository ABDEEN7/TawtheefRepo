namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed record JobCandidatePointsBreakdownDto
{
    public int CategoryPoints { get; init; }
    public int EducationPoints { get; init; }
    public int ExperiencePoints { get; init; }
    public int TrainingPoints { get; init; }
    public int SkillPoints { get; init; }
    public int LanguagePoints { get; init; }
    public int CertificatePoints { get; init; }
    public int TotalPoints { get; init; }
}