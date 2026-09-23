using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.DTOs;

public sealed record ResultReportDto(
    Guid Id,
    string Code,
    Guid InterviewScheduleId,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    string JobNameAr,
    string? JobNameEn,
    decimal? AppliedQualificationScore,
    ResultReportStatus Status,
    Guid? ApprovedById,
    DateTimeOffset? ApprovedAt,
    string? DecisionNotes,
    List<ResultCandidateDto> Candidates);

public sealed record ResultReportListItemDto(
   Guid Id,
    string Code,
    Guid InterviewScheduleId,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    string JobNameAr,
    string? JobNameEn,
    decimal? AppliedQualificationScore,
    ResultReportStatus Status,
    int CandidateCount,
    DateTimeOffset? ApprovedAt);
