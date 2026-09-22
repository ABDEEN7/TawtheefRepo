using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// All four session fields are derived from the schedule's live slots (superseded/cancelled rows excluded):
// First/LastSessionDate bound the days the schedule runs (equal for a single-day schedule)
public sealed record ScheduleListItemDto(
    Guid Id,
    Guid JobId,
    Guid? JobTitleId,
    string? JobTitleNameAr,
    string? JobTitleNameEn,
    string? CommitteeNameAr,
    string? CommitteeNameEn,
    DateOnly? FirstSessionDate,
    // here we return last session even if its not assigned yet.
    DateOnly? LastSessionDate, 
    TimeOnly? EarliestStartTime,
    TimeOnly? LatestEndTime,
    InterviewType DefaultInterviewType,
    int CandidatesCount,
    ScheduleStatus Status);
