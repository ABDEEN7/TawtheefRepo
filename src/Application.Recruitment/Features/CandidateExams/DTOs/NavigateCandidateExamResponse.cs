namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record NavigateCandidateExamResponse(Guid TestAttemptId, Guid TestAttemptPartId,
    int CurrentQuestionNumber, int TotalQuestionsInPart, bool HasPrevious, bool HasNext,
    bool AllowPreviousQuestion, bool WasChanged);
