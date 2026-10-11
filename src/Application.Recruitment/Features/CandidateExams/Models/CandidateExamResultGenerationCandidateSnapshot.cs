namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamResultGenerationCandidateSnapshot(
    Guid TestSessionCandidateId,
    Guid InvitationId,
    Guid TestSessionId,
    Guid TestSessionStatusId,
    Guid AttendanceStatusId,
    Guid? TestAttemptId,
    Guid? TestAttemptStatusId,
    bool HasOpenInterruption);
