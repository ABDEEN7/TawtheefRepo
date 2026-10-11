using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.TestSessions;

public sealed record TestSessionSubmittedForApprovalDomainEvent(Guid TestSessionId, string SessionNo)
    : BaseEvent(DateTimeOffset.UtcNow);
