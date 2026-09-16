using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// Everything the wizard's step 1/2 needs for a Job: identity fields, the linked (already-created)
// Committee and its roster, and the eligible-candidate pool stats for the step-2 cards.
public sealed record ScheduleCreationContextDto(
    Guid JobId,
    string JobTitleNameAr,
    string? JobTitleNameEn,
    string? MajorNameAr,
    string? MajorNameEn,
    string? DepartmentNameAr,
    string? DepartmentNameEn,
    Guid InterviewTemplateId,
    string InterviewTemplateTitleAr,
    string? InterviewTemplateTitleEn,
    bool InterviewTemplateHasApprovedVersion,
    Guid InterviewCommitteeId,
    string CommitteeNameAr,
    string? CommitteeNameEn,
    CommitteeStatus CommitteeStatus,
    List<ScheduleCommitteeMemberDto> Members,
    int EligibleCandidateCount,
    int EligibleMaleCount,
    int EligibleFemaleCount);

public sealed record ScheduleCommitteeMemberDto(
    Guid MemberUserId,
    string FullNameAr,
    string? FullNameEn,
    CommitteeRole Role);
