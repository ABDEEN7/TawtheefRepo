namespace Application.Recruitment.Features.CandidateExams.Models;

public sealed record CandidateExamCategoryScore(
    Guid QuestionBankTypeId,
    decimal WeightPercent,
    decimal Score);
