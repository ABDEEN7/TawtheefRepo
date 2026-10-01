namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record ReadyTestSlotsSummaryDto(
    int ExistingExamSessionCount,
    int ExistingExamCandidateCount,
    int AvailableCapacity);
