using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetOperationsDashboardQueryHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    ILocalizationService localizationService)
    : IQueryHandler<GetOperationsDashboardQuery, Result<OperationsDashboardDto>>
{
    public async Task<Result<OperationsDashboardDto>> Handle(GetOperationsDashboardQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var from = request.FromDateUtc ?? now.AddDays(-30);
        var to = request.ToDateUtc ?? now;

        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var employeesQuery = userManager.Users
            .OfType<EmployeeUser>()
            .AsNoTracking()
            .Include(x => x.EmployeeProfile)
            .Where(x => !x.IsDeleted);

        var profileQuery = profileRepo.DbSet
            .AsNoTracking()
            .Include(x => x.TargetEntity)
            .Where(x => !x.IsDeleted && x.CreatedDate >= from && x.CreatedDate <= to);

        if (request.DepartmentId.HasValue)
            profileQuery = profileQuery.Where(x => x.TargetEntityId == request.DepartmentId);

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<UserProfileStatus>(request.Status, true, out var status))
            profileQuery = profileQuery.Where(x => x.Status == status);

        if (request.EmployeeId.HasValue)
            profileQuery = profileQuery.Where(x => assignmentRepo.DbSet.Any(a => a.UserProfileId == x.Id && a.EmployeeId == request.EmployeeId));

        if (string.Equals(request.CurrentRole, nameof(SystemRoleIds.DepartmentManager), StringComparison.OrdinalIgnoreCase) && request.CurrentUserId.HasValue)
        {
            var currentEmployee = await userManager.Users
                .OfType<EmployeeUser>()
                .AsNoTracking()
                .Include(x => x.EmployeeProfile)
                .FirstOrDefaultAsync(x => x.Id == request.CurrentUserId!.Value, ct);

            var currentDepartment = currentEmployee?.EmployeeProfile?.Department;
            if (!string.IsNullOrWhiteSpace(currentDepartment))
                employeesQuery = employeesQuery.Where(x => x.EmployeeProfile != null && x.EmployeeProfile.Department == currentDepartment);
        }

        var totalEmployees = await employeesQuery.CountAsync(ct);
        var activeEmployees = await employeesQuery.CountAsync(x => !x.IsBlocked, ct);

        var totalProfiles = await profileQuery.CountAsync(ct);
        var approvedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Approved, ct);
        var pendingProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Submitted || x.Status == UserProfileStatus.UnderReview, ct);
        var returnedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.RequiresUpdate, ct);
        var rejectedProfiles = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(x => x.Status == ReviewStatus.Rejected && x.UserProfile != null && x.UserProfile.CreatedDate >= from && x.UserProfile.CreatedDate <= to)
            .Select(x => x.UserProfileId)
            .Distinct()
            .CountAsync(ct);

        var todayStart = now.Date;
        var weekStart = todayStart.AddDays(-(int)todayStart.DayOfWeek);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var newToday = await profileRepo.DbSet.CountAsync(x => !x.IsDeleted && x.CreatedDate >= todayStart && x.CreatedDate <= now, ct);
        var newWeek = await profileRepo.DbSet.CountAsync(x => !x.IsDeleted && x.CreatedDate >= weekStart && x.CreatedDate <= now, ct);
        var newMonth = await profileRepo.DbSet.CountAsync(x => !x.IsDeleted && x.CreatedDate >= monthStart && x.CreatedDate <= now, ct);

        var avgApprovalHours = await profileQuery
            .Where(x => x.Status == UserProfileStatus.Approved && x.UpdatedDate != null)
            .Select(x => EF.Functions.DateDiffMinute(x.CreatedDate, x.UpdatedDate!.Value) / 60.0m)
            .DefaultIfEmpty(0)
            .AverageAsync(ct);

        var employeeIds = await employeesQuery.Select(x => x.Id).ToListAsync(ct);

        var assignmentsQuery = assignmentRepo.DbSet
            .AsNoTracking()
            .Include(x => x.UserProfile)
            .Where(x => employeeIds.Contains(x.EmployeeId));

        var totalAssignedTasks = await assignmentsQuery.CountAsync(ct);
        var completedTasks = await assignmentsQuery.CountAsync(x => x.UserProfile != null && x.UserProfile.Status == UserProfileStatus.Approved, ct);
        var remainingTasks = totalAssignedTasks - completedTasks;
        var overdueTasks = await assignmentsQuery.CountAsync(x =>
            x.IsActive
            && x.UserProfile != null
            && x.UserProfile.Status != UserProfileStatus.Approved
            && x.AssignedAtUtc <= DateTimeOffset.UtcNow.AddDays(-3), ct);

        var byStatusRaw = await profileQuery
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var byDepartment = await profileQuery
            .GroupBy(x => x.TargetEntity != null ? (x.TargetEntity.NameEn ?? x.TargetEntity.NameAr ?? "N/A") : "N/A")
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync(ct);

        var byPriority = await profileQuery
            .Select(x => new
            {
                Priority = x.Status == UserProfileStatus.Submitted ? "high" :
                    x.Status == UserProfileStatus.UnderReview ? "medium" : "low"
            })
            .GroupBy(x => x.Priority)
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var aging = await profileQuery
            .Select(x => new { Days = EF.Functions.DateDiffDay(x.CreatedDate, now) })
            .GroupBy(x => x.Days <= 1 ? "0-1d" : x.Days <= 3 ? "2-3d" : x.Days <= 7 ? "4-7d" : ">7d")
            .Select(g => new AgingBucketDto { Bucket = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var tasksByDepartment = await assignmentRepo.DbSet
            .AsNoTracking()
            .Where(x => employeeIds.Contains(x.EmployeeId))
            .Join(userManager.Users.OfType<EmployeeUser>().AsNoTracking().Include(x => x.EmployeeProfile),
                assignment => assignment.EmployeeId,
                employee => employee.Id,
                (assignment, employee) => employee.EmployeeProfile != null ? employee.EmployeeProfile.Department : "N/A")
            .GroupBy(x => x ?? "N/A")
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync(ct);

        var tasksByUrgency = await assignmentsQuery
            .Select(x => new
            {
                Urgency = x.AssignedAtUtc <= DateTimeOffset.UtcNow.AddDays(-3) ? "high" :
                    x.AssignedAtUtc <= DateTimeOffset.UtcNow.AddDays(-1) ? "medium" : "low"
            })
            .GroupBy(x => x.Urgency)
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var taskStatusStacked = new List<GroupCountDto>
        {
            new() { Label = "completed", Count = completedTasks },
            new() { Label = "remaining", Count = remainingTasks },
            new() { Label = "overdue", Count = overdueTasks }
        };

        var trendLabels = Enumerable.Range(0, 14)
            .Select(i => now.Date.AddDays(-13 + i))
            .ToList();

        var profileTrendRaw = await profileRepo.DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedDate >= trendLabels.First() && x.CreatedDate <= now)
            .GroupBy(x => x.CreatedDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var taskTrendRaw = await assignmentRepo.DbSet
            .AsNoTracking()
            .Where(x => employeeIds.Contains(x.EmployeeId) && x.AssignedAtUtc >= trendLabels.First())
            .GroupBy(x => x.AssignedAtUtc.UtcDateTime.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var employeeList = await employeesQuery.ToListAsync(ct);

        var assignmentStats = await assignmentsQuery
            .GroupBy(x => x.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Assigned = g.Count(),
                Active = g.Count(x => x.IsActive),
                Completed = g.Count(x => x.UserProfile != null && x.UserProfile.Status == UserProfileStatus.Approved),
                Remaining = g.Count(x => x.UserProfile == null || x.UserProfile.Status != UserProfileStatus.Approved),
                Overdue = g.Count(x => x.IsActive && x.UserProfile != null && x.UserProfile.Status != UserProfileStatus.Approved && x.AssignedAtUtc <= DateTimeOffset.UtcNow.AddDays(-3)),
                AvgHandling = g.Where(x => x.UnassignedAtUtc != null)
                    .Select(x => (decimal?)EF.Functions.DateDiffMinute(x.AssignedAtUtc.UtcDateTime, x.UnassignedAtUtc!.Value.UtcDateTime) / 60m)
                    .Average()
            })
            .ToListAsync(ct);

        var reviewStats = await reviewRepo.DbSet
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

        var assignmentLookup = assignmentStats.ToDictionary(x => x.EmployeeId);
        var reviewLookup = reviewStats.ToDictionary(x => x.EmployeeId);

        var allRows = employeeList.Select(employee =>
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
                DepartmentName = employee.EmployeeProfile?.Department,
                AssignedTasks = assigned,
                ActiveTasks = active,
                CompletedTasks = a?.Completed ?? 0,
                RemainingTasks = a?.Remaining ?? 0,
                OverdueTasks = a?.Overdue ?? 0,
                ProfilesReviewed = reviewed,
                ApprovalRate = approvalRate,
                RejectionRate = rejectionRate,
                AverageHandlingHours = Math.Round(a?.AvgHandling ?? 0, 2),
                AverageResponseHours = Math.Round(r?.AvgResponse ?? 0, 2),
                WorkloadRatio = totalAssignedTasks == 0 ? 0 : Math.Round(assigned * 100m / totalAssignedTasks, 2),
                WorkloadBalanceIndicator = active > 25 ? "High" : active < 5 ? "Low" : "Balanced",
                ProductivityScore = Math.Round((approved * 0.6m) + ((a?.Completed ?? 0) * 0.4m), 2)
            };
        }).ToList();

        var teamCount = allRows.Count;
        var teamRows = allRows
            .OrderByDescending(x => x.ProductivityScore)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var topPerformers = allRows
            .OrderByDescending(x => x.ProductivityScore)
            .Take(5)
            .Select(x => new PerformanceRankDto { EmployeeId = x.EmployeeId, Name = x.Name, Score = x.ProductivityScore })
            .ToList();

        var underPerformers = allRows
            .Where(x => x.OverdueTasks > 0 || x.WorkloadBalanceIndicator == "High")
            .OrderBy(x => x.ProductivityScore)
            .Take(5)
            .Select(x => new PerformanceRankDto { EmployeeId = x.EmployeeId, Name = x.Name, Score = x.ProductivityScore })
            .ToList();

        var dto = new OperationsDashboardDto
        {
            Role = request.CurrentRole ?? nameof(SystemRoleIds.Employee),
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
                AverageApprovalHours = Math.Round(avgApprovalHours, 2),
                TotalAssignedTasks = totalAssignedTasks,
                RemainingTasks = remainingTasks,
                OverdueTasks = overdueTasks,
            },
            ProfileBreakdown = new ProfileBreakdownDto
            {
                ByStatus = byStatusRaw.Select(x => new StatusCountDto { Status = x.Status.ToString(), Count = x.Count }).ToList(),
                ByDepartment = byDepartment,
                ByPriority = byPriority,
                Aging = aging,
            },
            TaskMonitoring = new TaskMonitoringDto
            {
                TasksByDepartment = tasksByDepartment,
                TasksByUrgency = tasksByUrgency,
                TaskStatusStacked = taskStatusStacked,
            },
            ProfileTrend = new TrendSeriesDto
            {
                Points = trendLabels.Select(d => new TrendPointDto
                {
                    Label = d.ToString("dd MMM"),
                    Value = profileTrendRaw.FirstOrDefault(x => x.Date == d)?.Count ?? 0
                }).ToList()
            },
            TaskCompletionTrend = new TrendSeriesDto
            {
                Points = trendLabels.Select(d => new TrendPointDto
                {
                    Label = d.ToString("dd MMM"),
                    Value = taskTrendRaw.FirstOrDefault(x => x.Date == d)?.Count ?? 0
                }).ToList()
            },
            TeamPerformance = new PaginatedResult<TeamPerformanceRowDto>(teamRows, teamCount, request.PageNumber, request.PageSize),
            TopPerformers = topPerformers,
            UnderPerformers = underPerformers,
        };

        return Result.Ok(dto);
    }
}
