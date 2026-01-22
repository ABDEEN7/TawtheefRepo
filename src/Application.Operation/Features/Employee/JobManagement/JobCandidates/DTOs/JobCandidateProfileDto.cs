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

public sealed record JobCandidateProfileDto
{
    public Guid CandidateId { get; init; }
    public string? CandidateName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? NationalNumber { get; init; }
    public int? Age { get; init; }
    public string? CandidateType { get; init; }
    public string? Gender { get; init; }
    public string? Nationality { get; init; }
    public double ExperienceYears { get; init; }
    public JobCandidatePointsBreakdownDto Points { get; init; } = new();
}
