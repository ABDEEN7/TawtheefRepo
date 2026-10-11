namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamAssignmentValidationData(
    Guid Id,
    Guid TestSessionId,
    Guid ExamId,
    bool InvitationIsAccepted,
    DateTime? InvitationAcceptedAt,
    Guid InvitationStatusId,
    Guid ExamStatusId,
    Guid CandidateStatusId,
    Guid SlotStatusId,
    Guid SessionStatusId,
    Guid AttendanceStatusId,
    Guid IdentityStatusId,
    DateOnly SlotDate,
    TimeOnly? SessionStartTime,
    TimeOnly? SessionEndTime,
    int ExamDurationMinutes,
    string StoredAccessCode);
