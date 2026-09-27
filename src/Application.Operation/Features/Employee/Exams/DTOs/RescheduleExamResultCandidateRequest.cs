namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record RescheduleExamResultCandidateRequest(
    Guid TargetTestSessionId,
    string? Reason);
