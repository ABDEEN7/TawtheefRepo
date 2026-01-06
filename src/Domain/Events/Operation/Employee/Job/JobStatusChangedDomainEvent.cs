using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Job;

public sealed record JobStatusChangedDomainEvent(
    Guid JobId,
    Guid NewStatusId,
    DateTimeOffset DateOccurred) : BaseEvent(DateOccurred);
