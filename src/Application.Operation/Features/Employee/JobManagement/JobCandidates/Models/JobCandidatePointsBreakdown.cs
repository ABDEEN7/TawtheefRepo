namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public sealed record JobCandidatePointsBreakdown(
    int CategoryPoints,
    int EducationPoints,
    int ExperiencePoints,
    int TrainingPoints,
    int SkillPoints,
    int LanguagePoints,
    int CertificatePoints)
{
    public List<PointDetailDto> Details { get; init; } = new();

    public int TotalPoints =>
        CategoryPoints +
        EducationPoints +
        ExperiencePoints +
        TrainingPoints +
        SkillPoints +
        LanguagePoints +
        CertificatePoints;
}
