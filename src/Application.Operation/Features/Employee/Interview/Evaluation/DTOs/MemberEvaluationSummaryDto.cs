using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.DTOs;

// Status/TotalScore/SubmittedAt are null when the member has never saved a draft for this appointment.
public sealed record MemberEvaluationSummaryDto(
    Guid InterviewCommitteeMemberId,
    Guid MemberUserId,
    string MemberFullNameAr,
    string? MemberFullNameEn,
    CommitteeRole Role,
    MemberEvaluationStatus? Status,
    decimal? TotalScore,
    DateTime? SubmittedAt);

public sealed record AppointmentEvaluationSummaryDto(
    Guid InterviewAppointmentId,
    AppointmentStatus AppointmentStatus,
    List<MemberEvaluationSummaryDto> Members);
