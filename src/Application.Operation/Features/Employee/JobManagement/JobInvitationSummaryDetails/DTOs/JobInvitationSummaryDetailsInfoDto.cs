using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsInfoDto
{
    public Guid JobId { get; init; }
    public string JobName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DropdownOptions JobStatus { get; set; } = new();
    public DateTimeOffset ClosingDate { get; set; }
    public Guid? CurrentBatchNumber { get; set; }
}
