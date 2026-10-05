using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.DTOs;

// One row per schedule whose result report the caller may open in the Committee Head Review - drives
// the "Committee Review" action on the Start Interview sessions list.
public sealed record CommitteeReviewListItemDto(
    Guid InterviewScheduleId,
    Guid ReportId,
    string Code,
    ResultReportStatus Status);

// Header of the evaluation-form structure (template axes and their criteria), so each candidate's
// member scores can be laid out per criterion.
public sealed record CommitteeReviewCriterionDto(Guid CriterionId, string? NameAr, string? NameEn, decimal MaxScore);

public sealed record CommitteeReviewAxisDto(
    Guid AxisId,
    string? NameAr,
    string? NameEn,
    decimal MaxScore,
    decimal? QualificationScore,
    List<CommitteeReviewCriterionDto> Criteria);

public sealed record CommitteeReviewCriterionScoreDto(Guid CriterionId, decimal Score);

// A committee member's evaluation of one candidate. Scores are only listed for a submitted evaluation
// (the same set the result calculation uses); Status/TotalScore are null when nothing was saved.
public sealed record CommitteeReviewMemberDto(
    Guid InterviewCommitteeMemberId,
    string MemberFullNameAr,
    string? MemberFullNameEn,
    CommitteeRole Role,
    MemberEvaluationStatus? Status,
    decimal? TotalScore,
    DateTimeOffset? SubmittedAt,
    List<CommitteeReviewCriterionScoreDto> Scores);

public sealed record CommitteeReviewCandidateDto(
    Guid Id,
    Guid InterviewAppointmentId,
    string CandidateNameAr,
    string? CandidateNameEn,
    string? CandidateQid,
    decimal FinalScore,
    decimal? QualificationScore,
    bool IsQualified,
    AttendanceStatus? AttendanceStatus,
    bool IsLate,
    // Computed by ResultCandidateSuggestionService, the same as on the approval screen.
    FinalDecision SuggestedDecision,
    FinalDecision? ChairRecommendedDecision,
    string? ChairRecommendationReason,
    Guid? RecommendedSchoolStageId,
    List<OperationalIssueDto> OperationalIssues,
    List<ResultCandidateAxisDto> Axes,
    // Average of the submitted members' scores per criterion - display only, the stored axis scores
    // and final score are not recalculated from it.
    List<CommitteeReviewCriterionScoreDto> CriterionAverages,
    List<CommitteeReviewMemberDto> Members);

public sealed record CommitteeReviewDto(
    Guid ReportId,
    string Code,
    Guid InterviewScheduleId,
    string ScheduleTitleAr,
    string? ScheduleTitleEn,
    string JobNameAr,
    string? JobNameEn,
    decimal? AppliedQualificationScore,
    ResultReportStatus Status,
    DateTimeOffset? CommitteeReviewedAt,
    // Chair (or HR bypass) and the report is not approved yet.
    bool CanEdit,
    // CanEdit plus the InterviewCommitteeReview.OverrideSuggestion permission.
    bool CanOverrideSuggestion,
    List<CommitteeReviewAxisDto> Axes,
    List<CommitteeReviewCandidateDto> Candidates);
