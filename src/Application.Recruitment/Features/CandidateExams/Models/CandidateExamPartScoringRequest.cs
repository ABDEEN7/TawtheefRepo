namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamPartScoringRequest(
    Guid TestAttemptId,
    Guid TestAttemptPartId,
    Guid ExamPartId);
