using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Exams;

public sealed record CandidateExamOperationalIssueReportedDomainEvent(
    Guid InterruptionId,
    Guid TestAttemptId,
    Guid TestSlotId) : BaseEvent(DateTimeOffset.UtcNow);
