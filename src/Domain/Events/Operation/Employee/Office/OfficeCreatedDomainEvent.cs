using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Office;

public sealed record OfficeCreatedDomainEvent(Guid OfficeId, Guid AdminId, DateTimeOffset DateOccurred)
    : BaseEvent(DateOccurred);
