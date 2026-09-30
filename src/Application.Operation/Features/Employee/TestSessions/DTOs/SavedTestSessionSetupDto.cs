namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record SavedTestSessionSetupDto(
    Guid TestSessionId,
    string SessionNo,
    Guid StatusId,
    int CandidateCount);
