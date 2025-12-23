using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class DecideProfileReviewItemRequest
{
    public ReviewStatus Status { get; set; }
    public string? Note { get; set; }
}

