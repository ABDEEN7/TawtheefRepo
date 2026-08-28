namespace Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;

public sealed class CandidateCohortsDto
{
    public bool IncludeOfficeProfiles { get; init; }
    public int RegisteredKawaderProfiles { get; init; }
    public int RegisteredMinisterOfficeProfiles { get; init; }
    public int QatarGraduateProfiles { get; init; }
    public int OfficeProfiles { get; init; }
}
