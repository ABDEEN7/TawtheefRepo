using Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;
using Application.Operation.Features.Employee.Dashboard.DTOs.Common;
using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardOverviewReader(
    IUnitOfWork uow,
    ILocalizationService localizationService,
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
{
    private const int DefaultLookbackDays = 30;

    public async Task<Result<DashboardOverviewDto>> ReadAsync(DashboardQueryBase request, CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);
        var context = contextResult.Value;
        var profiles = scope.Profiles(request, context);
        var jobs = scope.Jobs(request, context);
        var assignments = scope.Assignments(context);
        var range = ResolveRange(request.FromDateUtc, request.ToDateUtc, DateTime.UtcNow);

        var candidateTypes = await GetCandidateTypesAsync(profiles, ct);
        var profileStatuses = await GetProfileStatusesAsync(profiles, ct);
        var jobStatuses = await GetJobStatusesAsync(jobs, ct);
        var tasks = await GetTaskAggregateAsync(assignments, ct);
        var awaitingDistribution = await CountUnassignedAsync(profiles, ct);
        var workload = await GetEmployeeWorkloadAsync(assignments, awaitingDistribution, ct);
        var employees = context.CanViewProfileDistribution
            ? await scope.DistributionTeam(context).CountAsync(ct)
            : 0;

        return Result.Ok(new DashboardOverviewDto
        {
            Role = context.CanViewProfileDistribution ? nameof(SystemRoleIds.HrManager)
                : context.CanViewAssignedProfiles ? nameof(SystemRoleIds.DepartmentManager)
                : nameof(SystemRoleIds.Employee),
            Filters = new DashboardFiltersSnapshotDto
            {
                FromDateUtc = request.FromDateUtc,
                ToDateUtc = request.ToDateUtc,
                DepartmentId = request.DepartmentId,
                EmployeeId = request.EmployeeId,
                Status = request.Status
            },
            Kpis = await BuildKpisAsync(profiles, profileStatuses, tasks, context, employees, awaitingDistribution, range, ct),
            ProfileBreakdown = new ProfileBreakdownDto
            {
                ByStatus = BuildStatusCounts(profileStatuses),
                ByCandidateType = candidateTypes
            },
            CandidateTypeKpis = BuildCandidateTypeKpis(candidateTypes),
            JobKpis = BuildJobKpis(jobStatuses),
            InvitationKpis = await BuildInvitationKpisAsync(request, context, ct),
            JobBreakdown = new JobBreakdownDto { ByStatus = BuildStatusCounts(jobStatuses) },
            TaskMonitoring = new TaskMonitoringDto
            {
                TaskStatusStacked =
                [
                    new GroupCountDto { Label = "ProfilesAwaitingDistribution", Count = workload.AwaitingDistribution },
                    new GroupCountDto { Label = "AssignedRequiringUpdate", Count = workload.Returned },
                    new GroupCountDto { Label = "CompletedAssignments", Count = workload.Completed },
                    new GroupCountDto { Label = "ActiveReviewWorkload", Count = workload.InProgress },
                    new GroupCountDto { Label = "AssignedSubmitted", Count = workload.Submitted }
                ]
            }
        });
    }

    private async Task<DashboardKpisDto> BuildKpisAsync(
        IQueryable<UserProfile> profiles,
        IReadOnlyDictionary<string, int> statuses,
        TaskCounts tasks,
        DashboardAccessContext context,
        int employees,
        int awaitingDistribution,
        (DateTime From, DateTime To) range,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var total = statuses.Values.Sum();
        var approved = statuses.GetValueOrDefault(nameof(UserProfileStatus.Approved));
        var rejected = await CountRejectedAsync(profiles, range.From, range.To, ct);
        var accessibleProfiles = scope.AccessibleProfiles(context);
        return new DashboardKpisDto
        {
            TotalEmployees = employees,
            ActiveEmployees = employees,
            TotalProfiles = total,
            NewProfilesToday = await CountNewProfilesAsync(accessibleProfiles, now.Date, now, ct),
            NewProfilesThisWeek = await CountNewProfilesAsync(accessibleProfiles, GetWeekStart(now.Date), now, ct),
            NewProfilesThisMonth = await CountNewProfilesAsync(accessibleProfiles, new DateTime(now.Year, now.Month, 1), now, ct),
            ApprovedProfiles = approved,
            RejectedProfiles = rejected,
            InCreationProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.InCreation)),
            SubmittedProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.Submitted)),
            UnderReviewProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.UnderReview)),
            PendingProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.Submitted)) + statuses.GetValueOrDefault(nameof(UserProfileStatus.UnderReview)),
            ReturnedProfiles = statuses.GetValueOrDefault(nameof(UserProfileStatus.RequiresUpdate)),
            ApprovalRate = total == 0 ? 0 : Math.Round(approved * 100m / total, 2),
            RejectionRate = total == 0 ? 0 : Math.Round(rejected * 100m / total, 2),
            AverageApprovalHours = await CalculateAverageApprovalHoursAsync(accessibleProfiles, range.From, range.To, ct),
            TotalAssignedTasks = tasks.Total,
            CompletedTasks = tasks.Completed,
            RemainingTasks = tasks.Remaining,
            OverdueTasks = tasks.Overdue,
            UnassignedProfiles = awaitingDistribution,
            FollowedMinisterOfficeCandidates = context.CanViewMinisterOffice
                ? await uow.GetEntityRepository<Tawtheef.Domain.Entities.MinisterOffice.MinisterOfficeCandidate>().DbSet
                    .CountAsync(x => !x.IsDeleted && x.IsFollowUpActive, ct)
                : 0
        };
    }

    private async Task<InvitationKpisDto> BuildInvitationKpisAsync(
        DashboardQueryBase request, DashboardAccessContext context, CancellationToken ct)
    {
        var rows = await scope.Invitations(request, context)
            .GroupBy(invitation => new { invitation.InvitationStatusId, invitation.IsAccepted })
            .Select(group => new { group.Key.InvitationStatusId, group.Key.IsAccepted, Count = group.Count() })
            .ToListAsync(ct);
        var total = rows.Sum(row => row.Count);
        var expired = rows.Where(row => !row.IsAccepted &&
            (row.InvitationStatusId == InvitationStatusIds.Closed ||
             row.InvitationStatusId == InvitationStatusIds.Cancelled ||
             row.InvitationStatusId == InvitationStatusIds.Expired)).Sum(row => row.Count);
        var rejected = rows.Where(row => !row.IsAccepted && row.InvitationStatusId == InvitationStatusIds.Rejected).Sum(row => row.Count);
        return new InvitationKpisDto
        {
            TotalInvitations = total,
            AcceptedInvitations = rows.Where(row => row.IsAccepted).Sum(row => row.Count),
            PendingInvitations = rows.Where(row => !row.IsAccepted &&
                row.InvitationStatusId != InvitationStatusIds.Rejected &&
                row.InvitationStatusId != InvitationStatusIds.Closed &&
                row.InvitationStatusId != InvitationStatusIds.Cancelled &&
                row.InvitationStatusId != InvitationStatusIds.Expired).Sum(row => row.Count),
            PendingAttachmentApproval = rows.Where(row => row.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval).Sum(row => row.Count),
            ExpiredInvitations = expired,
            RejectedInvitations = rejected
        };
    }

    private async Task<TaskCounts> GetTaskAggregateAsync(IQueryable<ProfileAssignment> assignments, CancellationToken ct)
    {
        var changes = uow.GetEntityRepository<ProfileChangeRequest>().DbSet.AsNoTracking();
        var row = await assignments.Where(x => x.IsActive && x.UnassignedAtUtc == null)
            .GroupBy(_ => 1)
            .Select(group => new TaskCounts(
                group.Select(x => x.UserProfileId).Distinct().Count(),
                group.Where(x => x.UserProfile!.Status == UserProfileStatus.Approved &&
                    !changes.Any(change => change.UserProfileId == x.UserProfileId &&
                        (change.Status == ProfileChangeRequestStatus.Pending || change.Status == ProfileChangeRequestStatus.UnderReview)))
                    .Select(x => x.UserProfileId).Distinct().Count(),
                group.Where(x => x.UserProfile!.Status != UserProfileStatus.Approved ||
                    changes.Any(change => change.UserProfileId == x.UserProfileId &&
                        (change.Status == ProfileChangeRequestStatus.Pending || change.Status == ProfileChangeRequestStatus.UnderReview)))
                    .Select(x => x.UserProfileId).Distinct().Count(),
                0))
            .FirstOrDefaultAsync(ct);
        return row ?? new TaskCounts(0, 0, 0, 0);
    }

    private async Task<EmployeeWorkloadCounts> GetEmployeeWorkloadAsync(
        IQueryable<ProfileAssignment> assignments, int awaitingDistribution, CancellationToken ct)
    {
        var changes = uow.GetEntityRepository<ProfileChangeRequest>().DbSet.AsNoTracking();
        var row = await assignments.Where(x => x.IsActive && x.UnassignedAtUtc == null)
            .Select(x => new
            {
                x.UserProfileId,
                x.UserProfile!.Status,
                HasPendingChange = changes.Any(change => change.UserProfileId == x.UserProfileId &&
                    (change.Status == ProfileChangeRequestStatus.Pending || change.Status == ProfileChangeRequestStatus.UnderReview))
            })
            .GroupBy(_ => 1)
            .Select(group => new EmployeeWorkloadCounts(
                0,
                group.Where(x => x.Status == UserProfileStatus.RequiresUpdate).Select(x => x.UserProfileId).Distinct().Count(),
                group.Where(x => x.Status == UserProfileStatus.Approved && !x.HasPendingChange).Select(x => x.UserProfileId).Distinct().Count(),
                group.Where(x => x.Status == UserProfileStatus.UnderReview || x.Status == UserProfileStatus.Approved && x.HasPendingChange).Select(x => x.UserProfileId).Distinct().Count(),
                group.Where(x => x.Status == UserProfileStatus.Submitted).Select(x => x.UserProfileId).Distinct().Count()))
            .FirstOrDefaultAsync(ct);
        return new EmployeeWorkloadCounts(awaitingDistribution, row?.Returned ?? 0,
            row?.Completed ?? 0, row?.InProgress ?? 0, row?.Submitted ?? 0);
    }

    private Task<int> CountUnassignedAsync(IQueryable<UserProfile> profiles, CancellationToken ct)
    {
        var assignments = uow.GetEntityRepository<ProfileAssignment>().DbSet;
        var changes = uow.GetEntityRepository<ProfileChangeRequest>().DbSet.AsNoTracking();
        return profiles.CountAsync(profile =>
            (Enumerable.Contains(ProfileDistributionRules.AssignableStatuses, profile.Status) ||
             profile.Status == UserProfileStatus.Approved && changes.Any(change => change.UserProfileId == profile.Id &&
                 (change.Status == ProfileChangeRequestStatus.Pending || change.Status == ProfileChangeRequestStatus.UnderReview))) &&
            !assignments.Any(assignment => assignment.UserProfileId == profile.Id && assignment.IsActive && assignment.UnassignedAtUtc == null), ct);
    }

    private Task<int> CountRejectedAsync(IQueryable<UserProfile> profiles, DateTime from, DateTime to, CancellationToken ct)
    {
        var allowedProfiles = profiles.Select(profile => profile.Id);
        return uow.GetEntityRepository<ReviewItem>().DbSet.AsNoTracking()
            .Where(item => item.Status == ReviewStatus.Rejected && item.UserProfile != null &&
                           allowedProfiles.Contains(item.UserProfileId) && item.UserProfile.CreatedDate >= from && item.UserProfile.CreatedDate <= to)
            .Select(item => item.UserProfileId).Distinct().CountAsync(ct);
    }

    private async Task<List<CandidateTypeCountDto>> GetCandidateTypesAsync(IQueryable<UserProfile> profiles, CancellationToken ct)
    {
        var rows = await profiles.GroupBy(profile => new
            {
                profile.CandidateTypeId,
                NameAr = profile.CandidateType != null ? profile.CandidateType.NameAr : string.Empty,
                NameEn = profile.CandidateType != null ? profile.CandidateType.NameEn : string.Empty
            })
            .Select(group => new { group.Key.CandidateTypeId, group.Key.NameAr, group.Key.NameEn, Count = group.Count() })
            .ToListAsync(ct);
        return rows.Select(row => new CandidateTypeCountDto
        {
            CandidateTypeId = row.CandidateTypeId,
            Key = GetCandidateTypeKey(row.CandidateTypeId),
            Label = row.CandidateTypeId.HasValue ? localizationService.GetLocalizedValue(row.NameAr, row.NameEn) : string.Empty,
            Count = row.Count
        }).ToList();
    }

    private static async Task<Dictionary<string, int>> GetProfileStatusesAsync(IQueryable<UserProfile> profiles, CancellationToken ct)
    {
        var rows = await profiles.GroupBy(profile => profile.Status)
            .Select(group => new { Status = group.Key.ToString(), Count = group.Count() }).ToListAsync(ct);
        return rows.ToDictionary(row => row.Status, row => row.Count, StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<Dictionary<string, int>> GetJobStatusesAsync(IQueryable<Job> jobs, CancellationToken ct)
    {
        var rows = await jobs.GroupBy(job => job.JobStatusId)
            .Select(group => new { Status = group.Key.ToString(), Count = group.Count() }).ToListAsync(ct);
        return rows.ToDictionary(row => row.Status, row => row.Count, StringComparer.OrdinalIgnoreCase);
    }

    private static CandidateTypeKpisDto BuildCandidateTypeKpis(IReadOnlyCollection<CandidateTypeCountDto> types)
    {
        var counts = types.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(v => v.Count), StringComparer.OrdinalIgnoreCase);
        int Count(string key) => counts.TryGetValue(key, out var value) ? value : 0;
        return new CandidateTypeKpisDto
        {
            Total = types.Sum(x => x.Count), Qatari = Count("qatari"), NonQatari = Count("nonQatari"),
            SonOfQatariMother = Count("sonOfQatariMother"), WifeOfQatari = Count("wifeOfQatari"), Gcc = Count("gcc"),
            ResidentQatar = Count("residentQatar"), Unknown = Count("unknown")
        };
    }

    private static JobKpisDto BuildJobKpis(IReadOnlyDictionary<string, int> statuses) => new()
    {
        TotalJobs = statuses.Values.Sum(),
        DraftJobs = statuses.GetValueOrDefault(JobStatusIds.Draft.ToString()),
        ActiveJobs = statuses.GetValueOrDefault(JobStatusIds.Published.ToString()),
        PendingReviewJobs = statuses.GetValueOrDefault(JobStatusIds.PendingApproval.ToString()),
        ApprovedJobs = statuses.GetValueOrDefault(JobStatusIds.PendingPointConfiguration.ToString()) + statuses.GetValueOrDefault(JobStatusIds.NeedPointUpdate.ToString()) + statuses.GetValueOrDefault(JobStatusIds.PendingPointApproval.ToString()),
        RejectedJobs = statuses.GetValueOrDefault(JobStatusIds.Rejected.ToString()),
        PendingPointConfigurationJobs = statuses.GetValueOrDefault(JobStatusIds.PendingPointConfiguration.ToString()),
        NeedPointUpdateJobs = statuses.GetValueOrDefault(JobStatusIds.NeedPointUpdate.ToString()),
        PendingPointApprovalJobs = statuses.GetValueOrDefault(JobStatusIds.PendingPointApproval.ToString()),
        NeedUpdateJobs = statuses.GetValueOrDefault(JobStatusIds.NeedUpdate.ToString()),
        ReadyForAnnouncementJobs = statuses.GetValueOrDefault(JobStatusIds.ReadyForAnnouncement.ToString()),
        PublishedJobs = statuses.GetValueOrDefault(JobStatusIds.Published.ToString()),
        ClosedJobs = statuses.GetValueOrDefault(JobStatusIds.Closed.ToString()),
        CancelledJobs = statuses.GetValueOrDefault(JobStatusIds.Cancelled.ToString())
    };

    private static List<StatusCountDto> BuildStatusCounts(IReadOnlyDictionary<string, int> statuses) =>
        statuses.Select(item => new StatusCountDto { Status = item.Key, Count = item.Value }).ToList();
    private static Task<int> CountNewProfilesAsync(IQueryable<UserProfile> profiles, DateTime from, DateTime to, CancellationToken ct) =>
        profiles.CountAsync(x => x.CreatedDate >= from && x.CreatedDate <= to, ct);
    private static async Task<decimal> CalculateAverageApprovalHoursAsync(IQueryable<UserProfile> profiles, DateTime from, DateTime to, CancellationToken ct)
    {
        var approvals = profiles.Where(x => x.CreatedDate >= from && x.CreatedDate <= to && x.Status == UserProfileStatus.Approved && x.UpdatedDate != null);
        var count = await approvals.CountAsync(ct);
        if (count == 0) return 0m;
        var hours = await approvals.Select(x => EF.Functions.DateDiffSecond(x.CreatedDate, x.UpdatedDate!.Value) / 3600.0).SumAsync(ct);
        return Math.Round((decimal)(hours / count), 2);
    }
    private static string GetCandidateTypeKey(Guid? id) => id == CandidateTypeIds.Qatari ? "qatari"
        : id == CandidateTypeIds.NonQatari ? "nonQatari" : id == CandidateTypeIds.SonOfQatariMother ? "sonOfQatariMother"
        : id == CandidateTypeIds.WifeOfQatari ? "wifeOfQatari" : id == CandidateTypeIds.GCC ? "gcc"
        : id == CandidateTypeIds.ResidentQatar ? "residentQatar" : id == CandidateTypeIds.QidHolder ? "qidHolder" : "unknown";
    private static (DateTime From, DateTime To) ResolveRange(DateTime? from, DateTime? to, DateTime now)
    {
        var resolved = (From: from ?? now.AddDays(-DefaultLookbackDays), To: to ?? now);
        return resolved.From <= resolved.To ? resolved : (resolved.To, resolved.From);
    }
    private static DateTime GetWeekStart(DateTime today) => today.AddDays(-(int)today.DayOfWeek);
    private sealed record TaskCounts(int Total, int Completed, int Remaining, int Overdue);
    private sealed record EmployeeWorkloadCounts(int AwaitingDistribution, int Returned, int Completed, int InProgress, int Submitted);
}
