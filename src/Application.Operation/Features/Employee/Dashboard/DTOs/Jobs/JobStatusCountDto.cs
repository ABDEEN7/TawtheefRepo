namespace Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

public sealed class JobStatusCountDto
{
    public Guid JobStatusId { get; init; }
    public required string Label { get; init; }
    public int Count { get; init; }
}
