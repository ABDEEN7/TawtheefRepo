namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsStatsDto
{
    public int Total { get; init; }
    public int New { get; init; }
    public int Applied { get; init; }
    public int Declined { get; init; }
    public int Cancelled { get; init; }
    public int Read { get; init; }
    public int Expired { get; init; }
    public int PendingAttachmentApproval { get; init; }
    public int ReturnedAttachment { get; init; }
}
