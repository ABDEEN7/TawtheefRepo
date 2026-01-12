namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

public sealed record PendingAttachmentSnapshot
{
    public Guid? AttachmentResourceId { get; init; }
    public required string Title { get; init; }
}