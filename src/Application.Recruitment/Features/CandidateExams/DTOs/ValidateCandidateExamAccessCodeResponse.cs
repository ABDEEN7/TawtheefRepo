namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record ValidateCandidateExamAccessCodeResponse(
    Guid TestSessionCandidateId,
    Guid TestSessionId,
    Guid ExamId,
    string EntryState);
