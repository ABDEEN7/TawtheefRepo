using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.DTOs;

public sealed record ResultCandidateDto(
    Guid Id,
    Guid InterviewAppointmentId,
    string CandidateNameAr,
    string? CandidateNameEn,
    // The candidate's personal ID (UserProfile.NationalNumber)
    string? CandidateQid,
    decimal FinalScore,
    decimal? QualificationScore,
    bool IsQualified,
    FinalDecision? FinalDecision,
    string? DecisionReason,
    // Dynamically computed at review time, It's not fixed => ResultCandidateSuggestionService : we can but preferred candidate logic there.
    FinalDecision SuggestedDecision,
    DateTimeOffset SnapshotAt,
    // So the Final Reviewer sees which candidates have operational issues without a separate call -
    // reuses the existing OperationalIssue feature's DTO rather than duplicating its shape.
    List<OperationalIssueDto> OperationalIssues,
    List<ResultCandidateAxisDto> Axes);



