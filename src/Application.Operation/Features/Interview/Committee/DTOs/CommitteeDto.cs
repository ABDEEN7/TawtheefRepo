using System;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.Committee.DTOs;

// CommitteeType* fields are derived read-only from Job.JobCategory - never stored on the committee itself.
public sealed record CommitteeDto(
    Guid Id,
    Guid JobId,
    string? JobTitleNameAr,
    string? JobTitleNameEn,
    Guid InterviewTemplateId,
    string InterviewTemplateTitleAr,
    string? InterviewTemplateTitleEn,
    Guid CommitteeTypeId,
    string CommitteeTypeNameAr,
    string? CommitteeTypeNameEn,
    string NameAr,
    string? NameEn,
    string? ScopeDescription,
    string? Notes,
    CommitteeStatus Status,
    bool IsActive,
    Guid? ApprovedById,
    DateTime? ApprovedAt,
    DateTime? ClosedAt,
    string? DecisionNotes);
