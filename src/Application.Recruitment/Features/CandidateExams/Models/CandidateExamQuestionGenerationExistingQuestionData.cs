namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamQuestionGenerationExistingQuestionData(
    Guid TestAttemptPartId,
    Guid ExamCategoryId,
    Guid QuestionId,
    Guid QuestionRevisionId,
    Guid DifficultyLevelId,
    int OrderNo,
    Guid? SelectedOptionId,
    DateTime? SavedAt);
