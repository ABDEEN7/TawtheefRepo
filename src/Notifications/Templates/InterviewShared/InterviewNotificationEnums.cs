namespace Tawtheef.Notifications.Templates.InterviewShared;

/// <summary>The interview workflow item a notification is about. The templates pick their wording from it.</summary>
public enum InterviewEntityKind
{
    Template = 1,
    Committee = 2,
    Schedule = 3,
    ResultReport = 4
}

public enum InterviewDecision
{
    Approved = 1,
    Returned = 2,
    Cancelled = 3
}
