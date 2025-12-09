using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.DTOs;

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
    public DateTimeOffset CreateDate { get; set; }
}
