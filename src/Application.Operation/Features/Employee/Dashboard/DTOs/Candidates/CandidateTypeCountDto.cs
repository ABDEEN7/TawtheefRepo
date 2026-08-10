namespace Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;

public sealed class CandidateTypeCountDto
{
    public Guid? CandidateTypeId { get; init; }
    public required string Key { get; init; }
    public required string Label { get; init; }
    public int Count { get; init; }
}
