namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed class JobCandidatesOverviewDto
{
    public int TotalCandidatesCount { get; init; }
    public int AvailableCandidatesCount { get; init; }
    public int AbovePointsCandidatesCount { get; init; }
    public double PointsAverage { get; init; }
}
