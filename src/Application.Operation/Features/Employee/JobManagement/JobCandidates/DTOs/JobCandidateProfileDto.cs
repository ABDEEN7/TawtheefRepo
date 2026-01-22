namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
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
