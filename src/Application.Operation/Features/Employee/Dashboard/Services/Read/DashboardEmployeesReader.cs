using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.Queries.Employees;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardEmployeesReader(
    IUnitOfWork uow,
    ILocalizationService localizationService,
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
{
    public async Task<Result<PaginatedResult<TeamPerformanceRowDto>>> ReadAsync(GetTeamPerformanceQuery request,
        CancellationToken ct)
    {
        var queryResult = await CreateQueryAsync(request, ct);
        if (queryResult.IsFailed) return Result.Fail(queryResult.Errors);

        var query = ApplySorting(queryResult.Value, request.SortBy, request.SortDirection);
        var total = await query.CountAsync(ct);
        var rows = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(ct);
        return Result.Ok(new PaginatedResult<TeamPerformanceRowDto>(rows, total, request.PageNumber, request.PageSize));
    }

    internal async Task<Result<IReadOnlyList<TeamPerformanceRowDto>>> ReadExportAsync(
        GetTeamPerformanceQuery request,
        DashboardAccessContext context,
        CancellationToken ct)
    {
        var queryResult = CreateQuery(request, context);
        if (queryResult.IsFailed) return Result.Fail(queryResult.Errors);

        var rows = await ApplySorting(queryResult.Value, request.SortBy, request.SortDirection).ToListAsync(ct);
        return Result.Ok<IReadOnlyList<TeamPerformanceRowDto>>(rows);
    }

    private async Task<Result<IQueryable<TeamPerformanceRowDto>>> CreateQueryAsync(
        GetTeamPerformanceQuery request, CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        return CreateQuery(request, contextResult.Value);
    }

    private Result<IQueryable<TeamPerformanceRowDto>> CreateQuery(
        GetTeamPerformanceQuery request,
        DashboardAccessContext context)
    {
        var employees = scope.DashboardEmployees(context);
        if (context.Scope != DashboardScope.User && request.EmployeeId.HasValue)
            employees = employees.Where(user => user.Id == request.EmployeeId.Value);
        var isArabic = localizationService.GetCurrentLanguage() == "ar";

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = $"%{request.Search.Trim()}%";
            employees = employees.Where(user =>
                EF.Functions.Like(user.FullNameAr, term) ||
                EF.Functions.Like(user.FullNameEn, term) ||
                user is EmployeeUser && (user as EmployeeUser)!.EmployeeProfile != null &&
                (EF.Functions.Like((user as EmployeeUser)!.EmployeeProfile!.EmployeeNumber ?? string.Empty, term) ||
                 EF.Functions.Like((user as EmployeeUser)!.EmployeeProfile!.Department ?? string.Empty, term) ||
                 EF.Functions.Like((user as EmployeeUser)!.EmployeeProfile!.JobTitle ?? string.Empty, term)));
        }

        var overdueCutoff = DashboardWorkloadRules.ResolveOverdueCutoff(DateTime.UtcNow);

        var range = DashboardTemporalResolver.ResolveRequestRange(
            request.Year,
            request.FromDateUtc,
            request.ToDateUtc,
            DateTime.UtcNow);

        var assignments = scope.Assignments(context, request.EmployeeId);

        var finalizedReviews = scope.CompletedReviews(context)
            .Where(log =>
                log.ActionType == UserProfileLogConstants.ActionTypes.ProfileReviewFinalized &&
                log.CreatedDate >= range.FromUtc &&
                log.CreatedDate < range.ToExclusiveUtc);

        var changes = uow.GetEntityRepository<ProfileChangeRequest>()
            .DbSet
            .AsNoTracking();

        var metrics = employees.Select(employee => new
        {
            Employee = employee,
            CompletedTasks = finalizedReviews.Count(log =>
                log.PerformedById == employee.Id),
            RemainingTasks = assignments
                .Where(x =>
                    x.EmployeeId == employee.Id &&
                    x.IsActive &&
                    x.UnassignedAtUtc == null &&
                    (x.UserProfile!.Status != UserProfileStatus.Approved ||
                     changes.Any(change =>
                         change.UserProfileId == x.UserProfileId &&
                         (change.Status == ProfileChangeRequestStatus.Pending ||
                          change.Status == ProfileChangeRequestStatus.UnderReview))))
                .Select(x => x.UserProfileId)
                .Distinct()
                .Count(),
            OverdueTasks = assignments
                .Where(x =>
                    x.EmployeeId == employee.Id &&
                    x.IsActive &&
                    x.UnassignedAtUtc == null &&
                    x.AssignedAtUtc < overdueCutoff &&
                    (x.UserProfile!.Status != UserProfileStatus.Approved ||
                     changes.Any(change =>
                         change.UserProfileId == x.UserProfileId &&
                         (change.Status == ProfileChangeRequestStatus.Pending ||
                          change.Status == ProfileChangeRequestStatus.UnderReview))))
                .Select(x => x.UserProfileId)
                .Distinct()
                .Count()
        });

        return Result.Ok(metrics.Select(x => new TeamPerformanceRowDto
        {
            EmployeeId = x.Employee.Id,
            Name = isArabic ? x.Employee.FullNameAr : x.Employee.FullNameEn,
            EmployeeNumber =
                x.Employee is EmployeeUser &&
                (x.Employee as EmployeeUser)!.EmployeeProfile != null
                    ? (x.Employee as EmployeeUser)!.EmployeeProfile!.EmployeeNumber
                    : null,
            DepartmentName =
                x.Employee is EmployeeUser &&
                (x.Employee as EmployeeUser)!.EmployeeProfile != null
                    ? (x.Employee as EmployeeUser)!.EmployeeProfile!.Department
                    : null,
            JobDescription =
                x.Employee is EmployeeUser &&
                (x.Employee as EmployeeUser)!.EmployeeProfile != null
                    ? (x.Employee as EmployeeUser)!.EmployeeProfile!.JobTitle
                    : null,
            CompletedTasks = x.CompletedTasks,
            RemainingTasks = x.RemainingTasks,
            OverdueTasks = x.OverdueTasks,
            AssignedTasks = x.CompletedTasks + x.RemainingTasks
        }));
    }

    private static IQueryable<TeamPerformanceRowDto> ApplySorting(
        IQueryable<TeamPerformanceRowDto> query,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(
            sortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase);

        return sortBy switch
        {
            nameof(TeamPerformanceRowDto.Name) => descending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),

            nameof(TeamPerformanceRowDto.EmployeeNumber) => descending
                ? query.OrderByDescending(x => x.EmployeeNumber)
                : query.OrderBy(x => x.EmployeeNumber),

            nameof(TeamPerformanceRowDto.AssignedTasks) => descending
                ? query.OrderByDescending(x => x.AssignedTasks)
                : query.OrderBy(x => x.AssignedTasks),

            nameof(TeamPerformanceRowDto.CompletedTasks) => descending
                ? query.OrderByDescending(x => x.CompletedTasks)
                : query.OrderBy(x => x.CompletedTasks),

            nameof(TeamPerformanceRowDto.RemainingTasks) => descending
                ? query.OrderByDescending(x => x.RemainingTasks)
                : query.OrderBy(x => x.RemainingTasks),

            nameof(TeamPerformanceRowDto.OverdueTasks) => descending
                ? query.OrderByDescending(x => x.OverdueTasks)
                : query.OrderBy(x => x.OverdueTasks),

            _ => query.OrderBy(x => x.Name)
        };
    }
}
