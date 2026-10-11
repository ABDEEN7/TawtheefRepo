namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamCurrentQuestionData(
    Guid Id,
    int OrderNo,
    string? Text,
    Guid? ResourceId,
    Guid? SelectedOptionId,
    DateTime? SavedAt,
    Guid QuestionTypeId,
    string QuestionTypeBackendName,
    string QuestionTypeName,
    string? QuestionTypeDescription,
    List<CandidateExamCurrentQuestionOptionData> Options);
