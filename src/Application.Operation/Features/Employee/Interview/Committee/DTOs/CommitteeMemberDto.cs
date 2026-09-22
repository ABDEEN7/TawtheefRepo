using System;
using System.Collections.Generic;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Committee.DTOs;

public sealed record CommitteeMemberDto(
    Guid Id,
    Guid InterviewCommitteeId,
    Guid MemberUserId,
    string MemberNameAr,
    string MemberNameEn,
    CommitteeRole Role,
    bool ParticipatesInEvaluation,
    EvaluationScope EvaluationScope,
    bool CanViewCandidates,
    bool CanAddNotes,
    bool CanSubmitEvaluation,
    bool CanViewOtherEvaluations,
    bool CanViewCommitteeSummary,
    bool IsActive,
    DateTimeOffset? RemovedAt,
    string? RemovalReason,
    List<CommitteeMemberEvaluationAxisDto> EvaluationAxes);
