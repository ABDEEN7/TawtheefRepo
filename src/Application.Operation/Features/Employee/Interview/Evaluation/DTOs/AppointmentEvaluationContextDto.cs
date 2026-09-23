using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.DTOs;

// "Who am I on this appointment's committee" - resolved without the GetMemberEvaluationFormQuery's
// CanSubmitEvaluation requirement, so a Chair who doesn't score can still be told they're the Chair.
// InterviewCommitteeMemberId/Role are null for a caller with no active membership row (HR/SuperAdmin
// bypass callers included) - HasChairOrBypassAccess is still meaningful for them via the bypass.
public sealed record AppointmentEvaluationContextDto(
    Guid? InterviewCommitteeMemberId,
    CommitteeRole? Role,
    bool CanSubmitEvaluation,
    bool CanViewCommitteeSummary,
    bool HasChairOrBypassAccess);
