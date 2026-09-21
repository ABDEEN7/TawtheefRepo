using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.DTOs;

public sealed record CandidateDecisionInputDto(Guid CandidateId, FinalDecision Decision, string? Reason);
