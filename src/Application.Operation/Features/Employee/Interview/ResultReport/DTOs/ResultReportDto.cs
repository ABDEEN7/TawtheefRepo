using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.DTOs;

public sealed record ResultReportDto(
    Guid Id,
    Guid InterviewScheduleId,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    string JobNameAr,
    string? JobNameEn,
    decimal? AppliedQualificationScore,
    ResultReportStatus Status,
    Guid? ApprovedById,
    DateTime? ApprovedAt,
    string? DecisionNotes,
    List<ResultCandidateDto> Candidates);

public sealed record ResultReportListItemDto(
    Guid Id,
    Guid InterviewScheduleId,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    string JobNameAr,
    string? JobNameEn,
    ResultReportStatus Status,
    int CandidateCount,
    DateTime? ApprovedAt);
