using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// Sessions = the schedule's live slots (superseded/cancelled rows excluded) folded into their contiguous
// sittings (ScheduleAppointmentPlanner.ReconstructPeriods), as UTC instants. The list's "session dates" and
// daily-hours envelope are derived from them client-side, because the calendar day and time of day depend on
// the viewer's timezone - the server can't compute them from UTC without guessing one.
public sealed record ScheduleListItemDto(
    Guid Id,
    Guid JobId,
    // JobTitleId lets the list's job filter group jobs the way the Job column shows them (by title).
    Guid? JobTitleId,
    string? JobTitleNameAr,
    string? JobTitleNameEn,
    // The job's one active committee - InterviewSchedule has no FK to it, same resolution as ScheduleDto.
    string? CommitteeNameAr,
    string? CommitteeNameEn,
    List<ScheduleSessionDto> Sessions,
    InterviewType DefaultInterviewType,
    int CandidatesCount,
    ScheduleStatus Status);

public sealed record ScheduleSessionDto(DateTimeOffset StartAt, DateTimeOffset EndAt);
