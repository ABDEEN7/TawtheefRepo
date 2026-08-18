namespace Application.Operation.Features.Employee.CandidateUsers.Contracts;

public enum CandidateUsersResultScope
{
    Default = 0,
    AccessibleProfiles = 1,
    [Obsolete("Use AccessibleProfiles. Retained for legacy query-string compatibility.")]
    DashboardAccessible = AccessibleProfiles
}
