using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.DTOs;

public sealed record ResultCandidateDto(
    Guid Id,
    Guid InterviewAppointmentId,
    string CandidateNameAr,
    string? CandidateNameEn,
    // The candidate's personal ID (UserProfile.NationalNumber) - same source as AppointmentDto.CandidateQid.
    string? CandidateQid,
    decimal FinalScore,
    decimal? QualificationScore,
    bool IsQualified,
    // Absent (NoShow) / Withdrew explain a 0 score; null when attendance was never recorded.
    AttendanceStatus? AttendanceStatus,
    // Arrived late (InterviewAppointment.IsLateCandidate) - shown next to the candidate because lateness
    // can affect selection priority.
    bool IsLate,
    FinalDecision? FinalDecision,
    string? DecisionReason,
    // Dynamically computed at review time, never persisted - see ResultCandidateSuggestionService.
    FinalDecision SuggestedDecision,
    // Committee Head Review - the chair's override of SuggestedDecision (null = kept the suggestion)
    // and the optional, informational recommended school stage.
    FinalDecision? ChairRecommendedDecision,
    string? ChairRecommendationReason,
    Guid? RecommendedSchoolStageId,
    string? RecommendedSchoolStageNameAr,
    string? RecommendedSchoolStageNameEn,
    DateTimeOffset SnapshotAt,
    // So the Final Reviewer sees which candidates have operational issues without a separate call -
    // reuses the existing OperationalIssue feature's DTO rather than duplicating its shape.
    List<OperationalIssueDto> OperationalIssues,
    List<ResultCandidateAxisDto> Axes);



