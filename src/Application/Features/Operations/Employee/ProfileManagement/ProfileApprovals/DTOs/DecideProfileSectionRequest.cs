using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;


public sealed class DecideProfileSectionRequest
{
    public ReviewStatus Status { get; set; } // Approved أو NeedsCorrection فقط في Full Review
    public string? Note { get; set; }
}
