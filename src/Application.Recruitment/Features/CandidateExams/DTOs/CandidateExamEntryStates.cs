namespace Application.Recruitment.Features.CandidateExams.DTOs;

public static class CandidateExamEntryStates
{
    public const string Assigned = nameof(Assigned);
    public const string NotEligible = nameof(NotEligible);
    public const string WaitingForSession = nameof(WaitingForSession);
    public const string EntryWindowClosed = nameof(EntryWindowClosed);
    public const string WaitingForAttendance = nameof(WaitingForAttendance);
    public const string WaitingForIdentityVerification = nameof(WaitingForIdentityVerification);
    public const string WaitingForAuthorization = nameof(WaitingForAuthorization);
    public const string ReadyToEnter = nameof(ReadyToEnter);
    public const string Started = nameof(Started);
    public const string NoShow = nameof(NoShow);
    public const string Completed = nameof(Completed);
    public const string Rescheduled = nameof(Rescheduled);
    public const string Cancelled = nameof(Cancelled);
    public const string Closed = nameof(Closed);
}
