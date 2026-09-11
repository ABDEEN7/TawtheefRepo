using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Exams;

public sealed record ExamApprovedDomainEvent(Guid CreatorId, Guid ExamId, string ExamNumber)
    : BaseEvent(DateTimeOffset.UtcNow);
