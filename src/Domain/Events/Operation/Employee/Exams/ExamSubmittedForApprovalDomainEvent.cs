using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Exams;

public sealed record ExamSubmittedForApprovalDomainEvent(
    Guid ExamId,
    string ExamNumber,
    string ExamTitleAr,
    string? ExamTitleEn,
    string JobTitleAr,
    string JobTitleEn) : BaseEvent(DateTimeOffset.UtcNow);
