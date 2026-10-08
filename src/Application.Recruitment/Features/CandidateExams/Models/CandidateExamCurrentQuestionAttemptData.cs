namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamCurrentQuestionAttemptData(
    Guid Id,
    Guid StatusId,
    Guid CandidateStatusId,
    Guid ExamId,
    bool AllowPreviousQuestion,
    Guid SessionStatusId,
    Guid? ActiveSessionId,
    Guid SlotStatusId,
    string JobTitle);
