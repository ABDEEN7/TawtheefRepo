using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.DTOs;

// EvaluationId is null until the caller's first SaveDraft - the form still renders (empty scores)
// against the template version even before any InterviewMemberEvaluation row exists.
public sealed record MemberEvaluationFormDto(
    Guid? EvaluationId,
    Guid InterviewAppointmentId,
    Guid InterviewCommitteeMemberId,
    MemberEvaluationStatus? Status,
    decimal? TotalScore,
    string? GeneralNotes,
    bool CanEdit,
    List<MemberEvaluationFormAxisDto> Axes);
