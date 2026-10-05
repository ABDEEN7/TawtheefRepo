public static class InterviewCommitteeAuditActions
{
    public const string Updated = "Updated";
    public const string MemberAdded = "MemberAdded";
    public const string MemberRemoved = "MemberRemoved";
    // System close once every schedule the committee sits on is finished (fired by result-report approval).
    public const string ClosedAutomatically = "ClosedAutomatically";
}
