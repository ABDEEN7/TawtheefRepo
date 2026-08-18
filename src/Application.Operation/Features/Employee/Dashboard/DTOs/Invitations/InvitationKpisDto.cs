namespace Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;

public sealed class InvitationKpisDto
{
    public int TotalInvitations { get; init; }
    public int AcceptedInvitations { get; init; }
    public int PendingInvitations { get; init; }
    public int PendingAttachmentApproval { get; init; }
    public int ExpiredInvitations { get; init; }
    public int RejectedInvitations { get; init; }
}
