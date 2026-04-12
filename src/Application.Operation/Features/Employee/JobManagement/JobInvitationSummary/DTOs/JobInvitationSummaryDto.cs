using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;

public class JobInvitationSummaryDto
{
    public Guid JobId { get; set; }
    public required string JobName { get; set; }
    public required string DepartmentName { get; set; }
    public required string JobCategory { get; set; }
    public required DropdownOptions JobStatus { get; set; }
    public required int InvitationCount { get; set; }
    public required int ApplicantsCount { get; set; }
    public required int RefusedCount { get; set; }
    public required int NotSeenCount { get; set; }
    public required int ReadCount { get; set; }
    public required int ExpiredCount { get; set; }
    public required int CancelledCount { get; set; }
    public required int PendingAttachmentApprovalCount { get; set; }
    public required int ReturnedAttachmentCount { get; set; }
    public required int PreviousBatchInvitations { get; set; }
    public required Guid? LastBatchNumber { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}
