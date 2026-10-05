using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.DTOs;

// RecommendedDecision null = keep the system suggestion.
public sealed record CommitteeReviewCandidateInputDto(
    Guid CandidateId, FinalDecision? RecommendedDecision, string? Reason, Guid? SchoolStageId);
