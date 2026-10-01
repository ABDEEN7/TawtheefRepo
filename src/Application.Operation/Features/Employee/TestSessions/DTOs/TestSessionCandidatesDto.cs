namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionCandidatesDto(
    IReadOnlyList<TestSessionCandidateListItemDto> Candidates,
    TestSessionCandidateSummaryDto Summary);
