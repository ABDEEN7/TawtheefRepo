namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record TimedOutCandidateExamPartDto(
    Guid TestAttemptPartId, int PartNo, DateTimeOffset TimedOutAt);
