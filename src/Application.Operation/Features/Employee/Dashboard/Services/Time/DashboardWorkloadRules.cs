namespace Application.Operation.Features.Employee.Dashboard.Services.Time;

internal static class DashboardWorkloadRules
{
    internal const int OverdueAfterDays = 7;

    internal static DateTime ResolveOverdueCutoff(DateTime utcNow) =>
        utcNow.AddDays(-OverdueAfterDays);
}
