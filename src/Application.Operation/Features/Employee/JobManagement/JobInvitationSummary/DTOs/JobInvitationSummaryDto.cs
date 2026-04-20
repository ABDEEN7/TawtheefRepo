using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;

public class JobInvitationSummaryDto
{
    public Guid JobId { get; init; }
    public required string JobName { get; set; }
    public required string DepartmentName { get; set; }
    public required string JobCategory { get; set; }
    public required DropdownOptions JobStatus { get; init; }
    public required int InvitationCount { get; init; }
    public required int ApplicantsCount { get; init; }
    public required int RefusedCount { get; init; }
    public required int NotSeenCount { get; init; }
    public required int ReadCount { get; init; }
    public required int ExpiredCount { get; init; }
    public required int CancelledCount { get; init; }
    public required int PendingAttachmentApprovalCount { get; set; }
    public required int ReturnedAttachmentCount { get; set; }
    public required int PreviousBatchInvitations { get; set; }
    public required Guid? LastBatchNumber { get; set; }
    public DateTimeOffset CreateDate { get; init; }
}
