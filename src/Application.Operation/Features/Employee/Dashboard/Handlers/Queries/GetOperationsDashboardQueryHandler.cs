using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetOperationsDashboardQueryHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    ILocalizationService localizationService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetOperationsDashboardQuery, Result<OperationsDashboardDto>>
{
    
    private const int DefaultLookbackDays = 30;
    private const int OverdueAfterDays = 3;
    private const int TrendDays = 14;
    private const int MaxTopItems = 8;
    private const int MaxRanks = 5;

    private async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken ct)
    {
        var roles = await userManager.GetRolesAsync(new User { Id = userId });
        if (!roles.Any()) return new List<string>();

        // Get permissions from RoleClaims
        var query = from rc in uow.Context.Set<IdentityRoleClaim<Guid>>().AsNoTracking()
                    join r in uow.Context.Set<ApplicationRole>().AsNoTracking() on rc.RoleId equals r.Id
                    where roles.Contains(r.Name!) && rc.ClaimType == "permission"
                    select rc.ClaimValue;

        return await query.Distinct().ToListAsync(ct) ?? new List<string>();
    }


    public async Task<Result<OperationsDashboardDto>> Handle(GetOperationsDashboardQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var range = ResolveRange(request, now);
        var todayStart = now.Date;
        var weekStart = GetWeekStart(todayStart);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        // ----------------------------
        // Scoping & Queries
        // ----------------------------
        var currentUserIdStr = currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserIdStr)) return Result.Fail("Unauthorized");
        var currentUserId = Guid.Parse(currentUserIdStr);

        var permissions = await GetUserPermissionsAsync(currentUserId, ct);

        var canViewAllProfiles = permissions.Contains(PermissionKeys.ProfileDistribution.View);
        var isDepartmentManager = permissions.Contains(PermissionKeys.ProfileApproval.Review);
        var hasMinisterOfficePermission = permissions.Contains(PermissionKeys.MinisterOffice.View);



        var repos = GetRepos(uow);

        var employeesQuery = BuildEmployeesQuery(userManager);
        var employeeIds = await employeesQuery.Select(x => x.Id).ToListAsync(ct);
        var employeeList = await employeesQuery.ToListAsync(ct);

        // 1) Resolve allowed country for current user (EmployeeUser => Qatar, OfficeUser => Office.CountryId)
        var allowedCountryId = await ResolveAllowedCountryIdAsync(currentUserId, ct);
        var profileQuery = BuildProfilesQuery(repos.Profile, request, repos.Assignment,
            employeeIds, canViewAllProfiles, allowedCountryId);
        var jobsQuery = BuildJobsQuery(repos.Job, request, employeeIds, canViewAllProfiles);


        // Trends (Still use the date range)
        var timedProfileQuery = repos.Profile.DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedDate >= range.From && x.CreatedDate <= range.To);
            
        if (!canViewAllProfiles)
        {
            timedProfileQuery = timedProfileQuery.Where(x =>
                repos.Assignment.DbSet.Any(a => a.UserProfileId == x.Id && employeeIds.Contains(a.EmployeeId)));
        }


        var totalEmployees = employeeList.Count;
        var activeEmployees = employeeList.Count(x => !x.IsBlocked);

        var followedCandidatesCount = 0;
        if (hasMinisterOfficePermission)
        {
            followedCandidatesCount = await repos.MinisterOfficeCandidate.DbSet
                .CountAsync(x => !x.IsDeleted && x.IsFollowUpActive, ct);
        }


        // Profiles KPIs (Current Pool)
        var totalProfiles = await profileQuery.CountAsync(ct);
        var approvedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Approved, ct);
        var pendingProfiles = await profileQuery.CountAsync(x =>
            x.Status == UserProfileStatus.Submitted || x.Status == UserProfileStatus.UnderReview, ct);
        var returnedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.RequiresUpdate, ct);

        // For rejected profiles, we might still want to look at the review date or just the current count
        var rejectedProfiles = await CountRejectedProfilesAsync(repos.Review, range.From, range.To, ct);

        // New profiles (global, not range-filtered) â€” keep same behavior as original code
        var newToday = await CountNewProfilesAsync(repos.Profile, todayStart, now, ct);
        var newWeek = await CountNewProfilesAsync(repos.Profile, weekStart, now, ct);
        var newMonth = await CountNewProfilesAsync(repos.Profile, monthStart, now, ct);

        // Approval time (avg hours) within selected range
        var avgApprovalHours = await CalculateAverageApprovalHoursAsync(repos.Profile, range.From, range.To, ct);

        // Assignments (scoped to employeeIds)
        var assignmentsQuery = BuildAssignmentsQuery(repos.Assignment, employeeIds, allowedCountryId);
        var baseAssignments = repos.Assignment.DbSet
            .AsNoTracking()
            .WhereIf(allowedCountryId is not null, a=> a.UserProfile!.ResidenceCountryId == allowedCountryId)
            .Where(a => !a.IsDeleted && employeeIds.Contains(a.EmployeeId));

        var latestAtPerProfile =
            from a in baseAssignments
            group a by a.UserProfileId into g
            select new
            {
                UserProfileId = g.Key,
                MaxAssignedAt = g.Max(x => x.AssignedAtUtc)
            };

        var latestIdPerProfile =
            from a in baseAssignments
            join m in latestAtPerProfile
                on new { a.UserProfileId, a.AssignedAtUtc }
                equals new { m.UserProfileId, AssignedAtUtc = m.MaxAssignedAt }
            group a by a.UserProfileId into g
            select new
            {
                UserProfileId = g.Key,
                AssignmentId = g.Max(x => x.Id)
            };

        var latestAssignments =
            from lid in latestIdPerProfile
            join a in baseAssignments on lid.AssignmentId equals a.Id
            join p in repos.Profile.DbSet.AsNoTracking().Where(p => !p.IsDeleted)
                on a.UserProfileId equals p.Id into pj
            from p in pj.DefaultIfEmpty()
            select new
            {
                a.AssignedAtUtc,
                a.UnassignedAtUtc,
                a.IsActive,
                ProfileStatus = (UserProfileStatus?)p.Status
            };

        var overdueCutoff = DateTimeOffset.UtcNow.AddDays(-OverdueAfterDays);

        // Job Metrics (already built and filtered)
        var totalJobs = await jobsQuery.CountAsync(ct);
        var activeJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Active || x.JobStatusId == JobStatusIds.Published, ct);
        var pendingReviewJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.PendingApproval, ct);
        var approvedJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.PendingPointConfiguration || x.JobStatusId == JobStatusIds.PendingPointApproval, ct);
        var rejectedJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Rejected, ct);
        var newJobsToday = await repos.Job.DbSet.CountAsync(x => !x.IsDeleted && x.CreatedDate >= todayStart, ct);

        var jobByStatusRaw = await jobsQuery
            .GroupBy(x => x.JobStatus != null ? x.JobStatus.BackendName : "N/A")
            .Select(g => new StatusCountDto { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var jobByManagementRaw = await jobsQuery
            .GroupBy(x => x.Management != null ? x.Management.NameEn : "N/A")
            .Select(g => new GroupCountDto { Label = Truncate(g.Key), Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(MaxTopItems)
            .ToListAsync(ct);

        var taskAgg = await latestAssignments
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(x=> x.ProfileStatus != UserProfileStatus.UnderReview || x.IsActive),
                Completed = g.Count(x =>
                    x.UnassignedAtUtc != null ||
                    x.ProfileStatus == UserProfileStatus.Approved ||
                    x.ProfileStatus == UserProfileStatus.RequiresUpdate),
                Overdue = g.Count(x =>
                    x.IsActive &&
                    x.UnassignedAtUtc == null &&
                    x.AssignedAtUtc <= overdueCutoff &&
                    x.ProfileStatus == UserProfileStatus.UnderReview),
                Remaining = g.Count(x => 
                    x.IsActive &&
                    (x.ProfileStatus == UserProfileStatus.UnderReview || x.ProfileStatus == UserProfileStatus.Submitted))
            })
            .OrderBy(x => 1)
            .FirstOrDefaultAsync(ct);

        var totalAssignedTasks = taskAgg?.Total ?? 0;
        var completedTasks = taskAgg?.Completed ?? 0;
        var overdueTasks = taskAgg?.Overdue ?? 0;
        var remainingTasks = taskAgg?.Remaining ?? 0;

        // Breakdown
        var byStatus = await GetProfilesByStatusAsync(profileQuery, ct);
        var byDepartment = await GetProfilesByTargetEntityAsync(profileQuery, ct);
        var byPriority = await GetProfilesByPriorityAsync(profileQuery, ct);
        var aging = await GetProfilesAgingAsync(profileQuery, now, ct);

        // Task monitoring breakdowns
        var tasksByDepartment = await GetTasksByDepartmentAsync(repos.Assignment, userManager, employeeIds, ct);
        var tasksByUrgency = await GetTasksByUrgencyAsync(assignmentsQuery, overdueCutoff, DateTimeOffset.UtcNow.AddDays(-1), ct);

        var taskStatusStacked = new List<GroupCountDto>
        {
            new() { Label = "common.completed", Count = completedTasks },
            new() { Label = "common.remaining", Count = remainingTasks },
            new() { Label = "common.overdue", Count = overdueTasks }
        };

        // Trends (last 14 days)
        var trendLabels = BuildTrendLabels(now, TrendDays);
        var trendStart = trendLabels[0]; // DateTime (00:00 UTC)
        var trendStartOffset = new DateTimeOffset(trendStart, TimeSpan.Zero);

        var profileTrendRaw = await GetProfileTrendAsync(repos.Profile, trendStart, now, ct);
        var taskTrendRaw = await GetTaskTrendAsync(repos.Assignment, employeeIds, trendStartOffset, trendStart, ct);

        // Team performance
        var assignmentStats = await GetAssignmentStatsAsync(assignmentsQuery, overdueCutoff, ct);
        var reviewStats = await GetReviewStatsAsync(repos.Review, employeeIds, ct);

        var allRows = BuildTeamRows(
            employeeList,
            assignmentStats,
            reviewStats,
            totalAssignedTasks,
            localizationService);

        var (teamRows, teamCount) = (allRows, allRows.Count);

        var topPerformers = allRows
            .OrderByDescending(x => x.ProductivityScore)
            .Take(MaxRanks)
            .Select(x => new PerformanceRankDto { EmployeeId = x.EmployeeId, Name = x.Name, Score = x.ProductivityScore })
            .ToList();

        var underPerformers = allRows
            .Where(x => x.OverdueTasks > 0 || x.WorkloadBalanceIndicator == "High")
            .OrderBy(x => x.ProductivityScore)
            .Take(MaxRanks)
            .Select(x => new PerformanceRankDto { EmployeeId = x.EmployeeId, Name = x.Name, Score = x.ProductivityScore })
            .ToList();

        var dto = new OperationsDashboardDto
        {
            Role = canViewAllProfiles ? "HrManager" : isDepartmentManager ? "DepartmentManager" : "Employee",



            Filters = new DashboardFiltersSnapshotDto
            {
                FromDateUtc = request.FromDateUtc,
                ToDateUtc = request.ToDateUtc,
                DepartmentId = request.DepartmentId,
                EmployeeId = request.EmployeeId,
                Status = request.Status
            },

            Kpis = new DashboardKpisDto
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,

                TotalProfiles = totalProfiles,
                NewProfilesToday = newToday,
                NewProfilesThisWeek = newWeek,
                NewProfilesThisMonth = newMonth,

                ApprovedProfiles = approvedProfiles,
                RejectedProfiles = rejectedProfiles,
                PendingProfiles = pendingProfiles,
                ReturnedProfiles = returnedProfiles,

                ApprovalRate = totalProfiles == 0 ? 0 : Math.Round(approvedProfiles * 100m / totalProfiles, 2),
                RejectionRate = totalProfiles == 0 ? 0 : Math.Round(rejectedProfiles * 100m / totalProfiles, 2),

                AverageApprovalHours = avgApprovalHours,

                TotalAssignedTasks = totalAssignedTasks,
                RemainingTasks = remainingTasks,
                OverdueTasks = overdueTasks,
                FollowedMinisterOfficeCandidates = followedCandidatesCount
            },

            ProfileBreakdown = new ProfileBreakdownDto
            {
                ByStatus = byStatus,
                ByDepartment = byDepartment,
                ByPriority = byPriority,
                Aging = aging,
            },

            JobKpis = new JobKpisDto
            {
                TotalJobs = totalJobs,
                ActiveJobs = activeJobs,
                PendingReviewJobs = pendingReviewJobs,
                ApprovedJobs = approvedJobs,
                RejectedJobs = rejectedJobs,
                NewJobsToday = newJobsToday
            },

            JobBreakdown = new JobBreakdownDto
            {
                ByStatus = jobByStatusRaw,
                ByDepartment = jobByManagementRaw
            },

            TaskMonitoring = new TaskMonitoringDto
            {
                TasksByDepartment = tasksByDepartment,
                TasksByUrgency = tasksByUrgency,
                TaskStatusStacked = taskStatusStacked,
            },

            ProfileTrend = new TrendSeriesDto
            {
                Points = BuildTrendPoints(trendLabels, profileTrendRaw)
            },

            TaskCompletionTrend = new TrendSeriesDto
            {
                Points = BuildTrendPoints(trendLabels, taskTrendRaw)
            },


            TopPerformers = topPerformers,
            UnderPerformers = underPerformers,
        };

        return Result.Ok(dto);
    }

    // ----------------------------
    // Query builders
    // ----------------------------

    private static IQueryable<EmployeeUser> BuildEmployeesQuery(UserManager<User> userManager) =>
        userManager.Users
            .OfType<EmployeeUser>()
            .AsNoTracking()
            .Include(x => x.EmployeeProfile);

    private static IQueryable<UserProfile> BuildProfilesQuery(
        IGenericRepository<UserProfile> profileRepo,
        GetOperationsDashboardQuery request,
        IGenericRepository<ProfileAssignment> assignmentRepo,
        IReadOnlyCollection<Guid> allowedEmployeeIds,
        bool isHrManager,
        Guid? allowedCountryId)
    {
        var query = profileRepo.DbSet
            .AsNoTracking()
            .Include(x => x.TargetEntity)
            .WhereIf(allowedCountryId is not null, x=> x.ResidenceCountryId == allowedCountryId)
            .Where(x => !x.IsDeleted);

        // Only apply dates if explicitly sent from UI
        if (request.FromDateUtc.HasValue)
            query = query.Where(x => x.CreatedDate >= request.FromDateUtc.Value);
        if (request.ToDateUtc.HasValue)
            query = query.Where(x => x.CreatedDate <= request.ToDateUtc.Value);

        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.TargetEntityId == request.DepartmentId);

        if (!string.IsNullOrWhiteSpace(request.Status)
            && Enum.TryParse<UserProfileStatus>(request.Status, true, out var status))
        {
            query = query.Where(x => x.Status == status);
        }

        // Apply Scoping
        if (!isHrManager)
        {
            query = query.Where(x =>
                assignmentRepo.DbSet.Any(a => a.UserProfileId == x.Id && allowedEmployeeIds.Contains(a.EmployeeId)));
        }
        else if (request.EmployeeId.HasValue)
        {
            var employeeId = request.EmployeeId.Value;
            query = query.Where(x =>
                assignmentRepo.DbSet.Any(a => a.UserProfileId == x.Id && a.EmployeeId == employeeId));
        }

        return query;
    }

    private static IQueryable<Job> BuildJobsQuery(
        IGenericRepository<Job> jobRepo,
        GetOperationsDashboardQuery request,
        IReadOnlyCollection<Guid> allowedEmployeeIds,
        bool isHrManager)
    {
        var query = jobRepo.DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (request.FromDateUtc.HasValue)
            query = query.Where(x => x.CreatedDate >= request.FromDateUtc.Value);
        if (request.ToDateUtc.HasValue)
            query = query.Where(x => x.CreatedDate <= request.ToDateUtc.Value);

        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == request.DepartmentId);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (string.Equals(request.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.JobStatusId == JobStatusIds.Active 
                                                 || x.JobStatusId == JobStatusIds.Published 
                                                 || x.JobStatusId == JobStatusIds.PendingPointConfiguration
                                                 || x.JobStatusId == JobStatusIds.PendingPointApproval);
            }
            else if (string.Equals(request.Status, "Submitted", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.JobStatusId == JobStatusIds.PendingApproval);
            }
            else if (string.Equals(request.Status, "RequiresUpdate", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.JobStatusId == JobStatusIds.NeedUpdate);
            }
            else if (string.Equals(request.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.JobStatusId == JobStatusIds.Rejected);
            }
        }

        return query;
    }

    private static IQueryable<ProfileAssignment> BuildAssignmentsQuery(
        IGenericRepository<ProfileAssignment> assignmentRepo,
        IReadOnlyCollection<Guid> employeeIds,
        Guid? allowedCountryId)
    {
        // Keep original behavior (Include UserProfile) because later queries reference UserProfile.Status
        return assignmentRepo.DbSet
            .AsNoTracking()
            .Include(x => x.UserProfile)
            .WhereIf(allowedCountryId is not null, x=> x.UserProfile!.ResidenceCountryId == allowedCountryId)
            .Where(x => employeeIds.Contains(x.EmployeeId));
    }


    // ----------------------------
    // Metrics helpers
    // ----------------------------

    private static async Task<int> CountRejectedProfilesAsync(
        IGenericRepository<ReviewItem> reviewRepo,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        return await reviewRepo.DbSet
            .AsNoTracking()
            .Where(x =>
                x.Status == ReviewStatus.Rejected
                && x.UserProfile != null
                && x.UserProfile.CreatedDate >= from
                && x.UserProfile.CreatedDate <= to)
            .Select(x => x.UserProfileId)
            .Distinct()
            .CountAsync(ct);
    }

    private static Task<int> CountNewProfilesAsync(
        IGenericRepository<UserProfile> profileRepo,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        return profileRepo.DbSet.CountAsync(x =>
            !x.IsDeleted && x.CreatedDate >= from && x.CreatedDate <= to, ct);
    }

    private static async Task<decimal> CalculateAverageApprovalHoursAsync(
        IGenericRepository<UserProfile> profileRepo,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var approvalsQuery = profileRepo.DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted
                && x.CreatedDate >= from
                && x.CreatedDate <= to
                && x.Status == UserProfileStatus.Approved
                && x.UpdatedDate != null);

        var approvalsCount = await approvalsQuery.CountAsync(ct);
        if (approvalsCount == 0) return 0m;

        var totalHours = await approvalsQuery
            .Select(x => EF.Functions.DateDiffSecond(x.CreatedDate, x.UpdatedDate!.Value) / 3600.0)
            .SumAsync(ct);

        return Math.Round((decimal)(totalHours / approvalsCount), 2);
    }

    // ----------------------------
    // Breakdown helpers
    // ----------------------------

    private static async Task<List<StatusCountDto>> GetProfilesByStatusAsync(
        IQueryable<UserProfile> profileQuery,
        CancellationToken ct)
    {
        var raw = await profileQuery
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return raw
            .Select(x => new StatusCountDto { Status = x.Status.ToString(), Count = x.Count })
            .ToList();
    }

    private static Task<List<GroupCountDto>> GetProfilesByTargetEntityAsync(
        IQueryable<UserProfile> profileQuery,
        CancellationToken ct)
    {
        return profileQuery
            .GroupBy(x => x.TargetEntity != null
                ? (x.TargetEntity.NameEn)
                : "N/A")
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(MaxTopItems)
            .ToListAsync(ct);
    }

    private static Task<List<GroupCountDto>> GetProfilesByPriorityAsync(
        IQueryable<UserProfile> profileQuery,
        CancellationToken ct)
    {
        // Same rule as original: Submitted=high, UnderReview=medium, else low
        return profileQuery
            .Select(x => new
            {
                Priority = x.Status == UserProfileStatus.Submitted ? "high"
                    : x.Status == UserProfileStatus.UnderReview ? "medium"
                    : "low"
            })
            .GroupBy(x => x.Priority)
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .ToListAsync(ct);
    }

    private static Task<List<AgingBucketDto>> GetProfilesAgingAsync(
        IQueryable<UserProfile> profileQuery,
        DateTime now,
        CancellationToken ct)
    {
        return profileQuery
            .Select(x => new { Days = EF.Functions.DateDiffDay(x.CreatedDate, now) })
            .GroupBy(x => x.Days <= 1 ? "0-1d"
                : x.Days <= 3 ? "2-3d"
                : x.Days <= 7 ? "4-7d"
                : ">7d")
            .Select(g => new AgingBucketDto { Bucket = g.Key, Count = g.Count() })
            .ToListAsync(ct);
    }

    // ----------------------------
    // Task monitoring helpers
    // ----------------------------

    private static Task<List<GroupCountDto>> GetTasksByDepartmentAsync(
        IGenericRepository<ProfileAssignment> assignmentRepo,
        UserManager<User> userManager,
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken ct)
    {
        return assignmentRepo.DbSet
            .AsNoTracking()
            .Where(x => employeeIds.Contains(x.EmployeeId))
            .Join(
                userManager.Users.OfType<EmployeeUser>().AsNoTracking().Include(x => x.EmployeeProfile),
                assignment => assignment.EmployeeId,
                employee => employee.Id,
                (_, employee) => employee.EmployeeProfile != null ? employee.EmployeeProfile.Department : "N/A")
            .GroupBy(x => x ?? "N/A")
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(MaxTopItems)
            .ToListAsync(ct);
    }

    private static Task<List<GroupCountDto>> GetTasksByUrgencyAsync(
        IQueryable<ProfileAssignment> assignmentsQuery,
        DateTimeOffset overdueCutoff,
        DateTimeOffset mediumCutoff,
        CancellationToken ct)
    {
        return assignmentsQuery
            .Select(x => new
            {
                Urgency = x.AssignedAtUtc <= overdueCutoff ? "high"
                    : x.AssignedAtUtc <= mediumCutoff ? "medium"
                    : "low"
            })
            .GroupBy(x => x.Urgency)
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .ToListAsync(ct);
    }

    // ----------------------------
    // Trends
    // ----------------------------

    private static List<DateTime> BuildTrendLabels(DateTime nowUtc, int days)
    {
        return Enumerable.Range(0, days)
            .Select(i => nowUtc.Date.AddDays(-(days - 1) + i))
            .ToList();
    }

    private static async Task<Dictionary<DateTime, int>> GetProfileTrendAsync(
        IGenericRepository<UserProfile> profileRepo,
        DateTime trendStart,
        DateTime now,
        CancellationToken ct)
    {
        var buckets = await profileRepo.DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedDate >= trendStart && x.CreatedDate <= now)
            .Select(x => new { Day = EF.Functions.DateDiffDay(trendStart, x.CreatedDate) })
            .GroupBy(x => x.Day)
            .Select(g => new { Day = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Convert to (date -> count) for O(1) lookups when building points
        return buckets.ToDictionary(
            x => trendStart.AddDays(x.Day),
            x => x.Count);
    }

    private static async Task<Dictionary<DateTime, int>> GetTaskTrendAsync(
        IGenericRepository<ProfileAssignment> assignmentRepo,
        IReadOnlyCollection<Guid> employeeIds,
        DateTimeOffset trendStartOffset,
        DateTime trendStart,
        CancellationToken ct)
    {
        var buckets = await assignmentRepo.DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted
                        && employeeIds.Contains(x.EmployeeId)
                        && x.AssignedAtUtc >= trendStartOffset)
            .Select(x => new { Day = EF.Functions.DateDiffDay(trendStartOffset, x.AssignedAtUtc) })
            .GroupBy(x => x.Day)
            .Select(g => new { Day = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return buckets.ToDictionary(
            x => trendStart.AddDays(x.Day),
            x => x.Count);
    }

    private static List<TrendPointDto> BuildTrendPoints(
        IReadOnlyList<DateTime> labels,
        IReadOnlyDictionary<DateTime, int> valuesByDate)
    {
        return labels.Select(d => new TrendPointDto
        {
            Label = d.ToString("dd MMM"),
            Value = valuesByDate.TryGetValue(d, out var v) ? v : 0
        }).ToList();
    }

    // ----------------------------
    // Team performance
    // ----------------------------

    private sealed record AssignmentStat(
        Guid EmployeeId,
        int Assigned,
        int Active,
        int Completed,
        int Remaining,
        int Overdue,
        decimal AvgHandlingHours);

    private sealed record ReviewStat(
        Guid EmployeeId,
        int Reviewed,
        int Approved,
        int Rejected,
        decimal AvgResponseHours);

    private static async Task<Dictionary<Guid, AssignmentStat>> GetAssignmentStatsAsync(
        IQueryable<ProfileAssignment> assignmentsQuery,
        DateTimeOffset overdueCutoff,
        CancellationToken ct)
    {
        var raw = await assignmentsQuery
            .GroupBy(x => x.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Assigned = g.Count(),
                Active = g.Count(x => x.IsActive),
                Completed = g.Count(x => x.UserProfile != null 
                                         && (x.UserProfile.Status == UserProfileStatus.Approved || x.UserProfile.Status == UserProfileStatus.RequiresUpdate)),
                Remaining = g.Count(x => x.UserProfile == null 
                                         || (x.UserProfile.Status == UserProfileStatus.UnderReview || x.UserProfile.Status == UserProfileStatus.Submitted)),
                Overdue = g.Count(x =>
                    x.IsActive
                    && x.UserProfile != null
                    && x.UserProfile.Status != UserProfileStatus.Approved
                    && x.AssignedAtUtc <= overdueCutoff),
                Times = g.Where(x => x.UnassignedAtUtc != null)
                    .Select(x => new { x.AssignedAtUtc, x.UnassignedAtUtc })
                    .ToList()
            })
            .ToListAsync(ct);

        var stats = raw.Select(x =>
        {
            var avgHandling = x.Times.Count == 0
                ? 0m
                : (decimal)x.Times.Average(t => (t.UnassignedAtUtc!.Value - t.AssignedAtUtc).TotalHours);

            return new AssignmentStat(
                x.EmployeeId,
                x.Assigned,
                x.Active,
                x.Completed,
                x.Remaining,
                x.Overdue,
                avgHandling);
        });

        return stats.ToDictionary(x => x.EmployeeId);
    }

    private static async Task<Dictionary<Guid, ReviewStat>> GetReviewStatsAsync(
        IGenericRepository<ReviewItem> reviewRepo,
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken ct)
    {
        var raw = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(x => x.ReviewedById != null && employeeIds.Contains(x.ReviewedById.Value))
            .GroupBy(x => x.ReviewedById!.Value)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Reviewed = g.Count(),
                Approved = g.Count(x => x.Status == ReviewStatus.Approved),
                Rejected = g.Count(x => x.Status == ReviewStatus.Rejected),
                AvgResponse = g.Where(x => x.ReviewedAtUtc != null)
                    .Select(x => (decimal?)EF.Functions.DateDiffMinute(x.CreatedDate, x.ReviewedAtUtc!.Value) / 60m)
                    .Average()
            })
            .ToListAsync(ct);

        return raw.Select(x => new ReviewStat(
                x.EmployeeId,
                x.Reviewed,
                x.Approved,
                x.Rejected,
                Math.Round(x.AvgResponse ?? 0m, 2)))
            .ToDictionary(x => x.EmployeeId);
    }

    private static List<TeamPerformanceRowDto> BuildTeamRows(
        IEnumerable<EmployeeUser> employees,
        IReadOnlyDictionary<Guid, AssignmentStat> assignmentLookup,
        IReadOnlyDictionary<Guid, ReviewStat> reviewLookup,
        int totalAssignedTasks,
        ILocalizationService localizationService)
    {
        return employees.Select(employee =>
        {
            assignmentLookup.TryGetValue(employee.Id, out var a);
            reviewLookup.TryGetValue(employee.Id, out var r);

            var reviewed = r?.Reviewed ?? 0;
            var approved = r?.Approved ?? 0;
            var rejected = r?.Rejected ?? 0;

            var approvalRate = reviewed == 0 ? 0 : Math.Round(approved * 100m / reviewed, 2);
            var rejectionRate = reviewed == 0 ? 0 : Math.Round(rejected * 100m / reviewed, 2);

            var assigned = a?.Assigned ?? 0;
            var active = a?.Active ?? 0;

            return new TeamPerformanceRowDto
            {
                EmployeeId = employee.Id,
                Name = localizationService.GetLocalizedFullName(employee),
                EmployeeNumber = employee.EmployeeProfile?.EmployeeNumber,
                DepartmentName = employee.EmployeeProfile?.Department,

                AssignedTasks = assigned,
                ActiveTasks = active,
                CompletedTasks = a?.Completed ?? 0,
                RemainingTasks = a?.Remaining ?? 0,
                OverdueTasks = a?.Overdue ?? 0,

                ProfilesReviewed = reviewed,
                ApprovalRate = approvalRate,
                RejectionRate = rejectionRate,

                AverageHandlingHours = Math.Round(a?.AvgHandlingHours ?? 0m, 2),
                AverageResponseHours = r?.AvgResponseHours ?? 0m,

                WorkloadRatio = totalAssignedTasks == 0 ? 0 : Math.Round(assigned * 100m / totalAssignedTasks, 2),
                WorkloadBalanceIndicator = active > 25 ? "High" : active < 5 ? "Low" : "Balanced",
                ProductivityScore = Math.Round((approved * 0.6m) + ((a?.Completed ?? 0) * 0.4m), 2)
            };
        }).ToList();
    }

    private static (List<T> Rows, int Total) Paginate<T>(List<T> all, int pageNumber, int pageSize)
    {
        var total = all.Count;
        var rows = all
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (rows, total);
    }

    // ----------------------------
    // Range & time helpers
    // ----------------------------

    private static (DateTime From, DateTime To) ResolveRange(GetOperationsDashboardQuery request, DateTime nowUtc)
    {
        var from = request.FromDateUtc ?? nowUtc.AddDays(-DefaultLookbackDays);
        var to = request.ToDateUtc ?? nowUtc;

        // Optional: guard against inverted ranges without throwing.
        if (from > to) (from, to) = (to, from);

        return (from, to);
    }

    private static DateTime GetWeekStart(DateTime todayStartUtc)
    {
        // Keep original behavior: Sunday-based week start (DayOfWeek Sunday = 0)
        return todayStartUtc.AddDays(-(int)todayStartUtc.DayOfWeek);
    }

    private static (IGenericRepository<UserProfile> Profile, IGenericRepository<ProfileAssignment> Assignment, 
        IGenericRepository<ReviewItem> Review, IGenericRepository<Job> Job,
        IGenericRepository<Tawtheef.Domain.Entities.MinisterOffice.MinisterOfficeCandidate> MinisterOfficeCandidate)
        GetRepos(IUnitOfWork uow)
    {
        return (
            uow.GetEntityRepository<UserProfile>(),
            uow.GetEntityRepository<ProfileAssignment>(),
            uow.GetEntityRepository<ReviewItem>(),
            uow.GetEntityRepository<Job>(),
            uow.GetEntityRepository<Tawtheef.Domain.Entities.MinisterOffice.MinisterOfficeCandidate>()
        );
    }
    
    public static string Truncate(string value, int maxLength = 20)
    {
        if (string.IsNullOrEmpty(value)) return value;

        return value.Length <= maxLength
            ? value
            : value.Substring(0, maxLength) + "...";
    }
    
    private async Task<Guid?> ResolveAllowedCountryIdAsync(Guid userId, CancellationToken ct)
    {
        // Load user + Office navigation safely for OfficeUser
        var user = await userManager.Users
            .Include(u => (u as OfficeUser)!.Office)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return null;

        return user switch
        {
            // EmployeeUser => Qatar only
            EmployeeUser => CountryIds.Qatar,

            // OfficeUser => Office.CountryId
            OfficeUser { Office: not null } officeUser => officeUser.Office.CountryId,

            _ => null
        };
    }
}

