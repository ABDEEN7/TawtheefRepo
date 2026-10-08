using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Exams;

public sealed record ExamResultReportReadyForReviewDomainEvent(
    Guid ReportId,
    Guid ExamId,
    int CandidateCount,
    DateTime TransitionAtUtc) : BaseEvent(DateTimeOffset.UtcNow);
