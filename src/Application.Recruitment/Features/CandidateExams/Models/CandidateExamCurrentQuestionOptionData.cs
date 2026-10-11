namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamCurrentQuestionOptionData(
    Guid Id, string? Text, int DisplayOrder, Guid? ResourceId);
