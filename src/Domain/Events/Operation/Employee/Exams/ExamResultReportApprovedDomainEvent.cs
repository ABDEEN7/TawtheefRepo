using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Exams;

public sealed record ExamResultReportApprovedDomainEvent(
    Guid ReportId,
    Guid ExamId,
    int QualifiedCandidateCount) : BaseEvent(DateTimeOffset.UtcNow);
