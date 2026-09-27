using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Exams;

public sealed record CandidateExamRescheduledDomainEvent(
    Guid AssignmentId,
    Guid ApplicantId,
    string ExamTitleAr,
    string? ExamTitleEn,
    DateOnly SlotDate,
    TimeOnly StartTime,
    string LocationAr,
    string? LocationEn,
    string RoomAr,
    string? RoomEn,
    int SessionNo) : BaseEvent(DateTimeOffset.UtcNow);
