using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.DTOs;

public sealed record ResultCandidateDto(
    Guid Id,
    Guid InterviewAppointmentId,
    string CandidateNameAr,
    string? CandidateNameEn,
    decimal FinalScore,
    decimal? QualificationScore,
    bool IsQualified,
    FinalDecision? FinalDecision,
    // Dynamically computed at review time, never persisted - see ResultCandidateSuggestionService.
    FinalDecision SuggestedDecision,
    DateTime SnapshotAt,
    // So the Final Reviewer sees which candidates have operational issues without a separate call -
    // reuses the existing OperationalIssue feature's DTO rather than duplicating its shape.
    List<OperationalIssueDto> OperationalIssues,
    List<ResultCandidateAxisDto> Axes);
