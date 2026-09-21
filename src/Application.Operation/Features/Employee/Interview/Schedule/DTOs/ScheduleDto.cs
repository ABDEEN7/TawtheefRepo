using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// InterviewCommittee* fields are resolved via JobId (Job -> its one active Committee), the same
// way CommitteeDto derives CommitteeType from Job.JobCategory - InterviewSchedule has no direct FK.
public sealed record ScheduleDto(
    Guid Id,
    Guid JobId,
    string? JobTitleNameAr,
    string? JobTitleNameEn,
    Guid InterviewTemplateId,
    string InterviewTemplateTitleAr,
    string? InterviewTemplateTitleEn,
    Guid? InterviewCommitteeId,
    string? CommitteeNameAr,
    string? CommitteeNameEn,
    string TitleAr,
    string? TitleEn,
    InterviewType DefaultInterviewType,
    int DefaultDurationMinutes,
    int DefaultBufferMinutes,
    ScheduleStatus Status,
    Guid? ApprovedById,
    DateTime? ApprovedAt,
    string? DecisionNotes,
    List<AppointmentDto> Appointments);
