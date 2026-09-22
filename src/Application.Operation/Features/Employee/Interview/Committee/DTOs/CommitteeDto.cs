using System;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Committee.DTOs;

// CommitteeType* fields are derived read-only from Job.JobCategory - never stored on the committee itself.
// Code is the database-generated committee number (COM-<year>-<number>); ChairName* is the active
// member whose role is Chair (null until one is assigned); MemberCount counts active members, Chair included.
public sealed record CommitteeDto(
    Guid Id,
    string Code,
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
    string? ChairNameAr,
    string? ChairNameEn,
    int MemberCount,
    string? ScopeDescription,
    string? Notes,
    CommitteeStatus Status,
    bool IsActive,
    Guid? ApprovedById,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? ClosedAt,
    string? DecisionNotes);
