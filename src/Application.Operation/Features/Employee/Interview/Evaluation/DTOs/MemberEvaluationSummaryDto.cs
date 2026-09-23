using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.DTOs;

// Status/TotalScore/SubmittedAt are null when the member has never saved a draft for this appointment.
// SubmittedAt is stored UTC and exposed as DateTimeOffset via AsUtcOffset() (see
// utc-datetimeoffset-convention) so the browser converts it to the viewer's local time instead of
// reading it as local and showing it hours early.
public sealed record MemberEvaluationSummaryDto(
    Guid InterviewCommitteeMemberId,
    Guid MemberUserId,
    string MemberFullNameAr,
    string? MemberFullNameEn,
    CommitteeRole Role,
    MemberEvaluationStatus? Status,
    decimal? TotalScore,
    DateTimeOffset? SubmittedAt);

public sealed record AppointmentEvaluationSummaryDto(
    Guid InterviewAppointmentId,
    AppointmentStatus AppointmentStatus,
    List<MemberEvaluationSummaryDto> Members);
