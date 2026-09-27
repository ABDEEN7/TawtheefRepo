namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record SaveCandidateExamAnswerResponse(Guid TestAttemptQuestionId, Guid SelectedOptionId,
    DateTime SavedAt, bool WasChanged);
