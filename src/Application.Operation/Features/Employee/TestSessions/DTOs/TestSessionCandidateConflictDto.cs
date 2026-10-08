namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionCandidateConflictDto(
    Guid InvitationId,
    string CandidateName,
    string? Qid);
