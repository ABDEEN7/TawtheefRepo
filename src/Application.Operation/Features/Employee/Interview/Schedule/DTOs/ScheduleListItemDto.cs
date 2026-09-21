using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

public sealed record ScheduleListItemDto(
    Guid Id,
    Guid JobId,
    string? JobTitleNameAr,
    string? JobTitleNameEn,
    DateOnly? FirstSessionDate,
    TimeOnly? FirstSessionTime,
    InterviewType DefaultInterviewType,
    int CandidatesCount,
    ScheduleStatus Status);
