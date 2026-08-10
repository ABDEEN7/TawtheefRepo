namespace Application.Operation.Features.Employee.Dashboard.DTOs.Common;

public sealed class StatusCountDto
{
    public required string Status { get; init; }
    public int Count { get; init; }
}
