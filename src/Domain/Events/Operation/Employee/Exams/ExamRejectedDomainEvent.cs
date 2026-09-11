using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Exams;

public sealed record ExamRejectedDomainEvent(Guid CreatorId, Guid ExamId, string ExamNumber, string Note)
    : BaseEvent(DateTimeOffset.UtcNow);
