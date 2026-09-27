namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CompletedCandidateExamPartDto(
    Guid TestAttemptPartId, int PartNo, DateTime CompletedAt);
