using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.TestSessions;

public sealed record TestSessionRejectedDomainEvent(
    Guid CreatorId,
    Guid TestSessionId,
    string SessionNo,
    string DecisionNote) : BaseEvent(DateTimeOffset.UtcNow);
