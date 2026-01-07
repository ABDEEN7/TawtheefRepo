using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsInfoDto
{
    public Guid JobId { get; init; }
    public string JobName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DropdownOptions JobStatus { get; set; } = new();
    public int CurrentBatchNumber { get; set; }
}
