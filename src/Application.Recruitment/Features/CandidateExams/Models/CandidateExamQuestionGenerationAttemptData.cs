namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamQuestionGenerationAttemptData(
    Guid Id, Guid StatusId, Guid ExamId, Guid ExamStatusId, int TotalQuestions);
