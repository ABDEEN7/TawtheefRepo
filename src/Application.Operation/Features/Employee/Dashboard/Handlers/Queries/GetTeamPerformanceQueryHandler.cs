using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetTeamPerformanceQueryHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IUserRepository userRepository,
    IMapper mapper,
    ILocalizationService localizationService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetTeamPerformanceQuery, Result<PaginatedResult<TeamPerformanceRowDto>>>
{
    private const int OverdueAfterDays = 3;

    public async Task<Result<PaginatedResult<TeamPerformanceRowDto>>> Handle(GetTeamPerformanceQuery request, CancellationToken ct)
    {
        var currentUserIdStr = currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserIdStr)) return Result.Fail("Unauthorized");
        var currentUserId = Guid.Parse(currentUserIdStr);

        var projection = new ProfileDistributionProjection(uow, userManager, userRepository, localizationService, mapper);
        var employees = await projection.LoadEmployeesAsync(currentUserId, ct);
        var employeeIds = employees.Select(x => x.EmployeeId).ToList();

        var employeeList = await userManager.Users
            .OfType<EmployeeUser>()
            .AsNoTracking()
            .Include(x => x.EmployeeProfile)
            .Where(x => employeeIds.Contains(x.Id))
            .ToListAsync(ct);
        
        // 1) Resolve allowed country for current user (EmployeeUser => Qatar, OfficeUser => Office.CountryId)
        var allowedCountryId = await ResolveAllowedCountryIdAsync(currentUserId, ct);
        var assignmentsQuery = uow.GetEntityRepository<ProfileAssignment>().DbSet
            .AsNoTracking()
            .Include(x => x.UserProfile)
            .WhereIf(allowedCountryId is not null, p=> p.UserProfile!.ResidenceCountryId == allowedCountryId)
            .Where(x => employeeIds.Contains(x.EmployeeId));

        var overdueCutoff = DateTimeOffset.UtcNow.AddDays(-OverdueAfterDays);
        
        var assignmentStats = await GetAssignmentStatsAsync(assignmentsQuery, overdueCutoff, ct);
        var reviewStats = await GetReviewStatsAsync(uow.GetEntityRepository<ReviewItem>(), employeeIds, ct);

        // Total assigned tasks for workload ratio
        var totalAssignedTasks = await assignmentsQuery.CountAsync(ct);

        var allRows = BuildTeamRows(
            employeeList,
            assignmentStats,
            reviewStats,
            totalAssignedTasks,
            localizationService);

        // Apply Search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            allRows = allRows.Where(x =>
                (x.Name != null && x.Name.ToLower().Contains(searchTerm)) ||
                (x.EmployeeNumber != null && x.EmployeeNumber.ToLower().Contains(searchTerm)) ||
                (x.DepartmentName != null && x.DepartmentName.ToLower().Contains(searchTerm)) ||
                (x.JobDescription != null && x.JobDescription.ToLower().Contains(searchTerm))
            ).ToList();
        }

        // Apply Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var isDesc = request.SortDirection?.ToLower() == "desc";
            allRows = request.SortBy switch
            {
                nameof(TeamPerformanceRowDto.Name) => isDesc ? allRows.OrderByDescending(x => x.Name).ToList() : allRows.OrderBy(x => x.Name).ToList(),
                nameof(TeamPerformanceRowDto.EmployeeNumber) => isDesc ? allRows.OrderByDescending(x => x.EmployeeNumber).ToList() : allRows.OrderBy(x => x.EmployeeNumber).ToList(),
                nameof(TeamPerformanceRowDto.AssignedTasks) => isDesc ? allRows.OrderByDescending(x => x.AssignedTasks).ToList() : allRows.OrderBy(x => x.AssignedTasks).ToList(),
                nameof(TeamPerformanceRowDto.CompletedTasks) => isDesc ? allRows.OrderByDescending(x => x.CompletedTasks).ToList() : allRows.OrderBy(x => x.CompletedTasks).ToList(),
                nameof(TeamPerformanceRowDto.OverdueTasks) => isDesc ? allRows.OrderByDescending(x => x.OverdueTasks).ToList() : allRows.OrderBy(x => x.OverdueTasks).ToList(),
                nameof(TeamPerformanceRowDto.ApprovalRate) => isDesc ? allRows.OrderByDescending(x => x.ApprovalRate).ToList() : allRows.OrderBy(x => x.ApprovalRate).ToList(),
                nameof(TeamPerformanceRowDto.AverageResponseHours) => isDesc ? allRows.OrderByDescending(x => x.AverageResponseHours).ToList() : allRows.OrderBy(x => x.AverageResponseHours).ToList(),
                nameof(TeamPerformanceRowDto.ProductivityScore) => isDesc ? allRows.OrderByDescending(x => x.ProductivityScore).ToList() : allRows.OrderBy(x => x.ProductivityScore).ToList(),
                _ => allRows
            };
        }

        var (teamRows, teamCount) = Paginate(allRows, request.PageNumber, request.PageSize);

        return Result.Ok(new PaginatedResult<TeamPerformanceRowDto>(teamRows, teamCount, request.PageNumber, request.PageSize));
    }

    private async Task<List<string>> GetUserPermissionsAsync(List<string> roles, CancellationToken ct)
    {
        if (!roles.Any()) return new List<string>();

        var query = from rc in uow.Context.Set<IdentityRoleClaim<Guid>>().AsNoTracking()
                    join r in uow.Context.Set<ApplicationRole>().AsNoTracking() on rc.RoleId equals r.Id
                    where roles.Contains(r.Name!) && rc.ClaimType == "permission"
                    select rc.ClaimValue;

        return await query.Distinct().ToListAsync(ct) ?? new List<string>();
    }

    private static (List<T> items, int total) Paginate<T>(List<T> source, int page, int size)
    {
        var total = source.Count;
        var items = source.Skip((page - 1) * size).Take(size).ToList();
        return (items, total);
    }

    private static List<TeamPerformanceRowDto> BuildTeamRows(
        List<EmployeeUser> employees,
        Dictionary<Guid, AssignmentStat> assignmentLookup,
        Dictionary<Guid, ReviewStat> reviewLookup,
        int globalTotalAssigned,
        ILocalizationService localizationService)
    {
        return employees.Select(employee =>
        {
            assignmentLookup.TryGetValue(employee.Id, out var assign);
            reviewLookup.TryGetValue(employee.Id, out var review);

            var assigned = assign?.Assigned ?? 0;
            var completed = assign?.Completed ?? 0;
            var active = assign?.Active ?? 0;
            var overdue = assign?.Overdue ?? 0;
            //var avgHandling = assign?.AvgHandlingHours ?? 0;

            var reviewed = review?.Reviewed ?? 0;
            var approved = review?.Approved ?? 0;
            //var avgResponse = review?.AvgResponseHours ?? 0;

            var approvalRate = reviewed == 0 ? 0 : Math.Round(approved * 100m / reviewed, 2);
            var rejectionRate = reviewed == 0 ? 0 : Math.Round((reviewed - approved) * 100m / reviewed, 2);

            var workloadRatio = globalTotalAssigned == 0 ? 0 : Math.Round(assigned * 100m / globalTotalAssigned, 2);

            var baseScore = (completed * 2) + (approved * 1);
            var penalty = (overdue * 3);
            //var productivityScore = Math.Max(0, baseScore - penalty);

            return new TeamPerformanceRowDto
            {
                EmployeeId = employee.Id,
                Name = localizationService.GetLocalizedFullName(employee),
                EmployeeNumber = employee.EmployeeProfile?.EmployeeNumber,
                DepartmentName = employee.EmployeeProfile?.Department,
                JobDescription = employee.EmployeeProfile?.JobTitle,

                AssignedTasks = assigned,
                ActiveTasks = active,
                CompletedTasks = completed,
                RemainingTasks = assign?.Remaining ?? 0,
                OverdueTasks = overdue,

                ProfilesReviewed = reviewed,
                ApprovalRate = approvalRate,
                RejectionRate = rejectionRate,
                // AverageHandlingHours = Math.Round(avgHandling, 2),
                // AverageResponseHours = Math.Round(avgResponse, 2),

                WorkloadRatio = workloadRatio,
                WorkloadBalanceIndicator = workloadRatio > 20 ? "High" : workloadRatio > 10 ? "Balanced" : "Low",
                // ProductivityScore = productivityScore
            };
        }).ToList();
    }

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
                Assigned = g.Count(x=> x.UserProfile!.Status != UserProfileStatus.UnderReview || x.IsActive),
                Active = g.Count(x => x.IsActive),
                Completed = g.Count(x => x.UserProfile != null 
                                         && (x.UserProfile.Status == UserProfileStatus.Approved || x.UserProfile.Status == UserProfileStatus.RequiresUpdate)),
                Remaining = g.Count(x =>  x.IsActive && (x.UserProfile == null 
                                         || (x.UserProfile.Status == UserProfileStatus.UnderReview))),
                Overdue = g.Count(x =>
                    x.IsActive
                    && x.UserProfile != null
                    && x.UserProfile.Status == UserProfileStatus.UnderReview
                    && x.AssignedAtUtc <= overdueCutoff),
                Times = g.Where(x => x.UnassignedAtUtc != null)
                    .Select(x => new { x.AssignedAtUtc, x.UnassignedAtUtc })
                    .ToList()
            })
            .ToListAsync(ct);

        var stats = raw.Select(x =>
        {
            // var avgHandling = x.Times.Count == 0
            //     ? 0m
            //     : (decimal)x.Times.Average(t => (t.UnassignedAtUtc!.Value - t.AssignedAtUtc).TotalHours);

            return new AssignmentStat(
                x.EmployeeId,
                x.Assigned,
                x.Active,
                x.Completed,
                x.Remaining,
                x.Overdue,
                0);
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
            .Where(x => employeeIds.Contains(x.UserProfile!.ProfileAssignments.OrderByDescending(a => a.AssignedAtUtc).First().EmployeeId))
            .GroupBy(x => x.UserProfile!.ProfileAssignments.OrderByDescending(a => a.AssignedAtUtc).First().EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Reviewed = g.Count(),
                Approved = g.Count(x => x.Status == ReviewStatus.Approved),
                Rejected = g.Count(x => x.Status == ReviewStatus.Rejected),
                Times = g.Select(x => new
                {
                    ReviewItemCreatedDate = x.CreatedDate, 
                    ReviewedAtUtcCreated = x.ReviewedAtUtc
                }).ToList()
            })
            .ToListAsync(ct);

        return raw.ToDictionary(x => x.EmployeeId, x =>
        {
            // var avgResponse = x.Times.Count == 0
            //     ? 0m
            //     : (decimal)x.Times.Average(t => 
            //         ((t.ReviewedAtUtcCreated ?? DateTime.UtcNow) - 
            //          t.ReviewItemCreatedDate).TotalHours);
            return new ReviewStat(x.EmployeeId, x.Reviewed, x.Approved, x.Rejected, 0);
        });
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
