using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class DecideProfileReviewItemRequest
{
    public ReviewStatus Status { get; set; }
    public string? Note { get; set; }
    public SpecializationRelationLevel? SpecializationRelation { get; set; }
}

