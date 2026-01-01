namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsStatsDto
{
    public int Total { get; init; }
    public int New { get; init; }
    public int Applied { get; init; }
    public int Declined { get; init; }
    public int Cancelled { get; init; }
}
