namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed class JobCandidatesOverviewDto
{
    public int TotalCandidatesCount { get; init; }
    public int AvailableCandidatesCount { get; init; }
    public int AvailableVacancies { get; init; }
    public double PointsAverage { get; init; }
}
