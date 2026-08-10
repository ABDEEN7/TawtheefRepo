namespace Application.Operation.Features.Employee.Dashboard.DTOs.Common;

public sealed class GroupCountDto
{
    public required string Label { get; init; }
    public int Count { get; init; }
}
