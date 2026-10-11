namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CandidateExamEntryValidationResult(Guid TestSessionCandidateId, Guid TestSessionId,
    Guid ExamId, Guid CandidateStatusId);
