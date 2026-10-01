namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionCandidateSummaryDto(
    int Total,
    int Eligible,
    int NotReady,
    int Excluded);
