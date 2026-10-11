namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamCurrentQuestionPartData(
    Guid Id,
    Guid StatusId,
    int PartNo,
    string Title,
    DateTime? StartedAt,
    int? DurationMinutes,
    DateTime? EffectiveDeadlineAt,
    int? LastQuestionNo,
    int TotalQuestions);
