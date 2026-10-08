namespace Application.Operation.Features.Employee.CandidateExamAttempts.DTOs;

public sealed record RescheduleInterruptedCandidateExamRequest(
    Guid TargetTestSessionId,
    string? Reason);
