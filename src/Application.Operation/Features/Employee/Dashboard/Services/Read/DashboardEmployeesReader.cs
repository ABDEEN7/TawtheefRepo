using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.Queries.Employees;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardEmployeesReader(
    IUnitOfWork uow,
    ILocalizationService localizationService,
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
{
    public async Task<Result<PaginatedResult<TeamPerformanceRowDto>>> ReadAsync(GetTeamPerformanceQuery request, CancellationToken ct)
    {
        var queryResult = await CreateQueryAsync(request, ct);
        if (queryResult.IsFailed) return Result.Fail(queryResult.Errors);
        if (!queryResult.Value.IsAuthorized)
            return Result.Ok(new PaginatedResult<TeamPerformanceRowDto>([], 0, request.PageNumber, request.PageSize));

        var query = ApplySorting(queryResult.Value.Query, request.SortBy, request.SortDirection);
        var total = await query.CountAsync(ct);
        var rows = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(ct);
        return Result.Ok(new PaginatedResult<TeamPerformanceRowDto>(rows, total, request.PageNumber, request.PageSize));
    }

    public async Task<Result<IReadOnlyList<TeamPerformanceRowDto>>> ReadExportAsync(GetTeamPerformanceQuery request, CancellationToken ct)
    {
        var queryResult = await CreateQueryAsync(request, ct);
        if (queryResult.IsFailed) return Result.Fail(queryResult.Errors);
        if (!queryResult.Value.IsAuthorized) return Result.Ok<IReadOnlyList<TeamPerformanceRowDto>>([]);

        var rows = await ApplySorting(queryResult.Value.Query, request.SortBy, request.SortDirection).ToListAsync(ct);
        return Result.Ok<IReadOnlyList<TeamPerformanceRowDto>>(rows);
    }

    private async Task<Result<TeamPerformanceQuery>> CreateQueryAsync(
        GetTeamPerformanceQuery request, CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);
        var context = contextResult.Value;
        if (!context.CanViewProfileDistribution)
            return Result.Ok(new TeamPerformanceQuery(false, Array.Empty<TeamPerformanceRowDto>().AsQueryable()));

        var employees = scope.DistributionTeam(context);
        if (request.EmployeeId.HasValue) employees = employees.Where(user => user.Id == request.EmployeeId.Value);
        var isArabic = localizationService.GetCurrentLanguage() == "ar";

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = $"%{request.Search.Trim()}%";
            employees = employees.Where(user =>
                EF.Functions.Like(isArabic ? user.FullNameAr : user.FullNameEn, term) ||
                user is EmployeeUser && (user as EmployeeUser)!.EmployeeProfile != null &&
                (EF.Functions.Like((user as EmployeeUser)!.EmployeeProfile!.EmployeeNumber ?? string.Empty, term) ||
                 EF.Functions.Like((user as EmployeeUser)!.EmployeeProfile!.Department ?? string.Empty, term) ||
                 EF.Functions.Like((user as EmployeeUser)!.EmployeeProfile!.JobTitle ?? string.Empty, term)));
        }

        var assignments = scope.Assignments(context);
        if (request.FromDateUtc.HasValue) assignments = assignments.Where(x => x.AssignedAtUtc >= request.FromDateUtc.Value);
        if (request.ToDateUtc.HasValue) assignments = assignments.Where(x => x.AssignedAtUtc <= request.ToDateUtc.Value);
        var changes = uow.GetEntityRepository<ProfileChangeRequest>().DbSet.AsNoTracking();
        return Result.Ok(new TeamPerformanceQuery(true, employees.Select(employee => new TeamPerformanceRowDto
        {
            EmployeeId = employee.Id,
            Name = isArabic ? employee.FullNameAr : employee.FullNameEn,
            EmployeeNumber = employee is EmployeeUser && (employee as EmployeeUser)!.EmployeeProfile != null
                ? (employee as EmployeeUser)!.EmployeeProfile!.EmployeeNumber : null,
            DepartmentName = employee is EmployeeUser && (employee as EmployeeUser)!.EmployeeProfile != null
                ? (employee as EmployeeUser)!.EmployeeProfile!.Department : null,
            JobDescription = employee is EmployeeUser && (employee as EmployeeUser)!.EmployeeProfile != null
                ? (employee as EmployeeUser)!.EmployeeProfile!.JobTitle : null,
            AssignedTasks = assignments.Where(x => x.EmployeeId == employee.Id && x.IsActive && x.UnassignedAtUtc == null)
                .Select(x => x.UserProfileId).Distinct().Count(),
            CompletedTasks = assignments.Where(x => x.EmployeeId == employee.Id && x.IsActive && x.UnassignedAtUtc == null &&
                    x.UserProfile!.Status == UserProfileStatus.Approved &&
                    !changes.Any(change => change.UserProfileId == x.UserProfileId &&
                        (change.Status == ProfileChangeRequestStatus.Pending || change.Status == ProfileChangeRequestStatus.UnderReview)))
                .Select(x => x.UserProfileId).Distinct().Count(),
            RemainingTasks = assignments.Where(x => x.EmployeeId == employee.Id && x.IsActive && x.UnassignedAtUtc == null &&
                    (x.UserProfile!.Status != UserProfileStatus.Approved ||
                     changes.Any(change => change.UserProfileId == x.UserProfileId &&
                         (change.Status == ProfileChangeRequestStatus.Pending || change.Status == ProfileChangeRequestStatus.UnderReview))))
                .Select(x => x.UserProfileId).Distinct().Count(),
            OverdueTasks = 0
        })));
    }

    private static IQueryable<TeamPerformanceRowDto> ApplySorting(
        IQueryable<TeamPerformanceRowDto> query, string? sortBy, string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return sortBy switch
        {
            nameof(TeamPerformanceRowDto.Name) => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            nameof(TeamPerformanceRowDto.EmployeeNumber) => descending ? query.OrderByDescending(x => x.EmployeeNumber) : query.OrderBy(x => x.EmployeeNumber),
            nameof(TeamPerformanceRowDto.AssignedTasks) => descending ? query.OrderByDescending(x => x.AssignedTasks) : query.OrderBy(x => x.AssignedTasks),
            nameof(TeamPerformanceRowDto.CompletedTasks) => descending ? query.OrderByDescending(x => x.CompletedTasks) : query.OrderBy(x => x.CompletedTasks),
            nameof(TeamPerformanceRowDto.OverdueTasks) => descending ? query.OrderByDescending(x => x.OverdueTasks) : query.OrderBy(x => x.OverdueTasks),
            _ => query.OrderBy(x => x.Name)
        };
    }

    private sealed record TeamPerformanceQuery(bool IsAuthorized, IQueryable<TeamPerformanceRowDto> Query);
}
