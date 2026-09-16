using System;
using System.Collections.Generic;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Committee.Commands;

// One row of the frontend's committee-member table. Role carries Chair directly (at most one row may
// have it) - there is no separate chair selector. EvaluationScope is not sent explicitly: an empty
// EvaluationAxisIds means this member evaluates all axes (the default, including for the Chair);
// a non-empty list scopes them to just those axes.
public sealed record MemberInputDto(
    Guid MemberUserId,
    CommitteeRole Role,
    bool ParticipatesInEvaluation,
    bool CanViewCandidates,
    bool CanAddNotes,
    bool CanSubmitEvaluation,
    bool CanViewOtherEvaluations,
    bool CanViewCommitteeSummary,
    List<Guid> EvaluationAxisIds);
