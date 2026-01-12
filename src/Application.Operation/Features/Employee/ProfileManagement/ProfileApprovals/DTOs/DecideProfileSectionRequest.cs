using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;


public sealed class DecideProfileSectionRequest
{
    public ReviewStatus Status { get; set; } // Approved أو NeedsCorrection فقط في Full Review
    public string? Note { get; set; }
}
