namespace Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

public sealed class InvitationStatusCountDto
{
    public required string Status { get; init; }
    public int Count { get; init; }
}
