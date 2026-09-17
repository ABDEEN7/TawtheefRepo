namespace Application.Operation.Features.Employee.Interview.Evaluation.DTOs;

// One criterion score from the member's scoring form. MaxScore is never accepted from the caller -
// the handler resolves it server-side from InterviewTemplateEvaluationCriterion before validating.
public sealed record CriterionScoreInputDto(Guid CriterionId, decimal Score, string? Notes);
