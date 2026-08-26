using Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;
using Application.Operation.Features.Employee.Dashboard.DTOs.Common;
using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using FluentResults;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardOverviewReader(
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope,
    DashboardProfileMetricsReader profileMetricsReader,
    DashboardJobMetricsReader jobMetricsReader,
    DashboardInvitationMetricsReader invitationMetricsReader,
    DashboardWorkloadMetricsReader workloadMetricsReader)
{
    public async Task<Result<DashboardOverviewDto>> ReadAsync(
        DashboardQueryBase request,
        CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        return await ReadAsync(request, contextResult.Value, ct);
    }

    internal async Task<Result<DashboardOverviewDto>> ReadAsync(
        DashboardQueryBase request,
        DashboardAccessContext context,
        CancellationToken ct)
    {
        var period = DashboardYearPeriod.FromRequest(request.Year, request.FromDateUtc);
        var requestRange = DashboardTemporalResolver.ResolveRequestRange(
            request.Year, request.FromDateUtc, request.ToDateUtc, DateTime.UtcNow);
        var profiles = scope.Profiles(request, context, requestRange);
        var periodProfiles = scope.ProfilesForPeriod(
            request, context, period.PreviousFrom, period.CurrentToExclusive);
        var periodJobs = scope.JobsForPeriod(
            request, context, period.PreviousFrom, period.CurrentToExclusive);
        var invitationJobs = scope.AccessibleJobs(context);
        if (request.DepartmentId.HasValue)
            invitationJobs = invitationJobs.Where(job => job.DepartmentId == request.DepartmentId.Value);
        var assignments = scope.Assignments(context, request.EmployeeId);
        var profileMetrics = await profileMetricsReader.ReadAsync(
            profiles,
            periodProfiles,
            context,
            period.CurrentFrom,
            (requestRange.FromUtc, requestRange.ToExclusiveUtc),
            ct);
        var jobMetrics = await jobMetricsReader.ReadAsync(periodJobs, period.CurrentFrom, ct);
        var workloadMetrics = await workloadMetricsReader.ReadAsync(
            assignments,
            context,
            request.EmployeeId,
            profileMetrics.Unassigned.Current,
            requestRange.FromUtc,
            requestRange.ToExclusiveUtc,
            ct);
        var invitationMetrics = await invitationMetricsReader.ReadAsync(
            invitationJobs,
            period.CurrentFrom,
            period.PreviousFrom,
            period.CurrentToExclusive,
            ct);

        var currentKpis = BuildKpis(profileMetrics, workloadMetrics);
        return Result.Ok(new DashboardOverviewDto
        {
            Kpis = currentKpis,
            ProfileBreakdown = new ProfileBreakdownDto
            {
                ByStatus = BuildStatusCounts(profileMetrics.Statuses.Current),
                ByCandidateType = profileMetrics.CandidateTypes,
                Cohorts = profileMetrics.Cohorts
            },
            JobKpis = jobMetrics.Kpis.Current,
            JobBreakdown = new JobBreakdownDto
            {
                ByStatus = jobMetrics.CurrentStatusBreakdown
            },
            InvitationKpis = invitationMetrics.Kpis.Current,
            KpiTrends = BuildKpiTrends(
                currentKpis,
                profileMetrics,
                jobMetrics.Kpis.Current,
                jobMetrics.Kpis.Previous,
                invitationMetrics.Kpis.Current,
                invitationMetrics.Kpis.Previous),
            TaskMonitoring = BuildTaskMonitoring(workloadMetrics.Workload)
        });
    }

    private static DashboardKpisDto BuildKpis(
        DashboardProfileMetrics profiles,
        DashboardWorkloadMetrics workload)
    {
        var statuses = profiles.Statuses.Current;
        var total = statuses.Values.Sum();
        var approved = statuses.GetValueOrDefault(nameof(UserProfileStatus.Approved));
        return new DashboardKpisDto
        {
            TotalEmployees = workload.Employees,
            ActiveEmployees = workload.Employees,
            TotalProfiles = total,
            NewProfilesToday = profiles.NewToday,
            NewProfilesThisWeek = profiles.NewThisWeek,
            NewProfilesThisMonth = profiles.NewThisMonth,
            ApprovedProfiles = approved,
            RejectedProfiles = profiles.Rejected,
            InCreationProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.InCreation)),
            SubmittedProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.Submitted)),
            UnderReviewProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.UnderReview)),
            ReturnedProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.RequiresUpdate)),
            ApprovalRate = total == 0 ? 0 : Math.Round(approved * 100m / total, 2),
            RejectionRate = total == 0 ? 0 : Math.Round(profiles.Rejected * 100m / total, 2),
            AverageApprovalHours = profiles.AverageApprovalHours,
            TotalAssignedTasks = workload.Tasks.Total,
            CompletedTasks = workload.Tasks.Completed,
            RemainingTasks = workload.Tasks.Remaining,
            OverdueTasks = workload.Tasks.Overdue,
            UnassignedProfiles = profiles.Unassigned.Current,
            FollowedMinisterOfficeCandidates = profiles.FollowedMinisterOfficeCandidates,
            KawaderFiles = profiles.KawaderFiles
        };
    }

    private static DashboardKpiTrendsDto BuildKpiTrends(
        DashboardKpisDto currentProfiles,
        DashboardProfileMetrics profileMetrics,
        JobKpisDto currentJobs,
        JobKpisDto previousJobs,
        InvitationKpisDto currentInvitations,
        InvitationKpisDto previousInvitations)
    {
        var previousStatuses = profileMetrics.Statuses.Previous;
        return new DashboardKpiTrendsDto
        {
            TotalProfiles = CalculateTrend(
                currentProfiles.TotalProfiles, previousStatuses.Values.Sum()),
            ApprovedProfiles = CalculateTrend(
                currentProfiles.ApprovedProfiles,
                previousStatuses.GetValueOrDefault(nameof(UserProfileStatus.Approved))),
            UnderReviewProfiles = CalculateTrend(
                currentProfiles.UnderReviewProfiles,
                previousStatuses.GetValueOrDefault(nameof(UserProfileStatus.UnderReview))),
            UnassignedProfiles = CalculateTrend(
                currentProfiles.UnassignedProfiles, profileMetrics.Unassigned.Previous),
            PublishedJobs = CalculateTrend(
                currentJobs.PublishedJobs, previousJobs.PublishedJobs),
            TotalInvitations = CalculateTrend(
                currentInvitations.TotalInvitations, previousInvitations.TotalInvitations),
            AcceptedInvitations = CalculateTrend(
                currentInvitations.AcceptedInvitations, previousInvitations.AcceptedInvitations)
        };
    }

    private static DashboardMetricTrendDto CalculateTrend(int current, int previous) => new()
    {
        PreviousValue = previous,
        ChangePercentage = previous == 0
            ? null
            : Math.Round((current - previous) * 100m / previous, 1)
    };

    private static List<StatusCountDto> BuildStatusCounts(
        IReadOnlyDictionary<string, int> statuses) =>
    [
        .. statuses.Select(item => new StatusCountDto { Status = item.Key, Count = item.Value })
    ];
    
    private static TaskMonitoringDto BuildTaskMonitoring(
        DashboardEmployeeWorkload workload) => new()
    {
        TaskStatusStacked =
        [
            new GroupCountDto
            {
                Label = "ProfilesAwaitingDistribution",
                Count = workload.AwaitingDistribution
            },
            new GroupCountDto
            {
                Label = "ActiveReviewWorkload",
                Count = workload.UnderReview
            },
            new GroupCountDto
            {
                Label = "CompletedReviews",
                Count = workload.CompletedReviews
            }
        ]
    };
}
