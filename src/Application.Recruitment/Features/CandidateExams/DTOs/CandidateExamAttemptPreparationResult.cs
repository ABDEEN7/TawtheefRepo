namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CandidateExamAttemptPreparationResult(
    Guid TestAttemptId,
    bool WasCreated,
    Guid StatusId,
    int PreparedPartCount);
