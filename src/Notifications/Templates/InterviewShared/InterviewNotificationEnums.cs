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

/// <summary>What happened to a committee member. Drives the wording of InterviewCommitteeAssignment.</summary>
public enum InterviewAssignmentChange
{
    /// <summary>Added to an approved committee (on approval, or added afterwards).</summary>
    Assigned = 1,
    /// <summary>Removed from an approved committee.</summary>
    Removed = 2,
    /// <summary>A schedule with interviews for the member's committee was approved.</summary>
    ScheduleApproved = 3,
    /// <summary>A schedule the member was told about (it had been approved) was cancelled.</summary>
    ScheduleCancelled = 4
}

/// <summary>What happened to a candidate's booking. Drives the wording of InterviewAppointmentChanged.</summary>
public enum InterviewAppointmentChange
{
    /// <summary>The interview moved to another date/time (and possibly place).</summary>
    Rescheduled = 1,
    /// <summary>The interview was cancelled - on its own or with its whole schedule.</summary>
    Cancelled = 2
}
