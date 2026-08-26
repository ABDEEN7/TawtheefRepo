using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardWorkloadMetricsReader(
    IUnitOfWork uow,
    DashboardQueryScope scope)
{
    public async Task<DashboardWorkloadMetrics> ReadAsync(
        IQueryable<ProfileAssignment> assignments,
        DashboardAccessContext context,
        Guid? requestedEmployeeId,
        int awaitingDistribution,
        DateTime fromUtc,
        DateTime toExclusiveUtc,
        CancellationToken ct)
    {
        var tasks = await GetTaskAggregateAsync(assignments, ct);

        var workload = await GetEmployeeWorkloadAsync(
            assignments,
            context,
            requestedEmployeeId,
            awaitingDistribution,
            fromUtc,
            toExclusiveUtc,
            ct);

        var employeesQuery = scope.DashboardEmployees(context);
        if (context.Scope != DashboardScope.User && requestedEmployeeId.HasValue)
            employeesQuery = employeesQuery.Where(employee => employee.Id == requestedEmployeeId.Value);

        var employees = await employeesQuery.CountAsync(ct);

        return new DashboardWorkloadMetrics(
            tasks,
            workload,
            employees);
    }

    private async Task<DashboardTaskCounts> GetTaskAggregateAsync(
        IQueryable<ProfileAssignment> assignments,
        CancellationToken ct)
    {
        var overdueCutoff = DashboardWorkloadRules.ResolveOverdueCutoff(DateTime.UtcNow);
        var changes = uow
            .GetEntityRepository<ProfileChangeRequest>()
            .DbSet
            .AsNoTracking();

        var row = await assignments
            .Where(assignment =>
                assignment.IsActive &&
                assignment.UnassignedAtUtc == null)
            .GroupBy(_ => 1)
            .Select(group => new DashboardTaskCounts(
                group
                    .Select(assignment => assignment.UserProfileId)
                    .Distinct()
                    .Count(),

                group
                    .Where(assignment =>
                        assignment.UserProfile!.Status == UserProfileStatus.Approved &&
                        !changes.Any(change =>
                            change.UserProfileId == assignment.UserProfileId &&
                            (change.Status == ProfileChangeRequestStatus.Pending ||
                             change.Status == ProfileChangeRequestStatus.UnderReview)))
                    .Select(assignment => assignment.UserProfileId)
                    .Distinct()
                    .Count(),

                group
                    .Where(assignment =>
                        assignment.UserProfile!.Status != UserProfileStatus.Approved ||
                        changes.Any(change =>
                            change.UserProfileId == assignment.UserProfileId &&
                            (change.Status == ProfileChangeRequestStatus.Pending ||
                             change.Status == ProfileChangeRequestStatus.UnderReview)))
                    .Select(assignment => assignment.UserProfileId)
                    .Distinct()
                    .Count(),

                group
                    .Where(assignment =>
                        assignment.IsActive &&
                        assignment.UnassignedAtUtc == null &&
                        assignment.AssignedAtUtc < overdueCutoff)
                    .Select(assignment => assignment.UserProfileId)
                    .Distinct()
                    .Count()))
            .FirstOrDefaultAsync(ct);

        return row ?? new DashboardTaskCounts(0, 0, 0, 0);
    }

    private async Task<DashboardEmployeeWorkload> GetEmployeeWorkloadAsync(
        IQueryable<ProfileAssignment> assignments,
        DashboardAccessContext context,
        Guid? requestedEmployeeId,
        int awaitingDistribution,
        DateTime fromUtc,
        DateTime toExclusiveUtc,
        CancellationToken ct)
    {
        var underReview = await assignments
            .Where(assignment =>
                assignment.IsActive &&
                assignment.UnassignedAtUtc == null &&
                assignment.UserProfile!.Status == UserProfileStatus.UnderReview)
            .Select(assignment => assignment.UserProfileId)
            .Distinct()
            .CountAsync(ct);

        var completed = await CountCompletedReviewsAsync(
            context,
            requestedEmployeeId,
            fromUtc,
            toExclusiveUtc,
            ct);

        return new DashboardEmployeeWorkload(
            awaitingDistribution,
            underReview,
            completed);
    }

    private async Task<int> CountCompletedReviewsAsync(
        DashboardAccessContext context,
        Guid? requestedEmployeeId,
        DateTime fromUtc,
        DateTime toExclusiveUtc,
        CancellationToken ct)
    {
        var logs = scope.CompletedReviews(context, requestedEmployeeId)
            .Where(log =>
                log.ActionType ==
                    UserProfileLogConstants.ActionTypes.ProfileReviewFinalized &&
                log.CreatedDate >= fromUtc &&
                log.CreatedDate < toExclusiveUtc);

        return await logs.CountAsync(ct);
    }
}
