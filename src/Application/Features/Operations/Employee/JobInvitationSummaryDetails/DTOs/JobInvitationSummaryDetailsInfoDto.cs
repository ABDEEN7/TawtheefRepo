namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsInfoDto
{
    public Guid JobId { get; init; }
    public string JobName { get; set; } = string.Empty;
}
