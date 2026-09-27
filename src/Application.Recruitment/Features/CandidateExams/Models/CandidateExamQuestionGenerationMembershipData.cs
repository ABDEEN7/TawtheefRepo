namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamQuestionGenerationMembershipData(
    Guid QuestionBankVersionId,
    Guid QuestionId,
    Guid QuestionRevisionId,
    Guid RevisionQuestionId,
    Guid DifficultyLevelId);
