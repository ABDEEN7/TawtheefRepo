namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CandidateExamQuestionGenerationResult(
    Guid TestAttemptId,
    bool WasGenerated,
    int TotalQuestionCount,
    int PartCount);
