namespace Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

public sealed class JobBreakdownDto
{
    public required IReadOnlyList<JobStatusCountDto> ByStatus { get; init; }
}
