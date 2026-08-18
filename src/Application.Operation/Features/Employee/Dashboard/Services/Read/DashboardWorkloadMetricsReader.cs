using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
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
        int awaitingDistribution,
        DateTime fromUtc,
        DateTime toExclusiveUtc,
        CancellationToken ct)
    {
        var tasks = await GetTaskAggregateAsync(assignments, ct);

        var workload = await GetEmployeeWorkloadAsync(
            assignments,
            context,
            awaitingDistribution,
            fromUtc,
            toExclusiveUtc,
            ct);

        var employees = context.CanViewProfileDistribution
            ? await scope.DistributionTeam(context).CountAsync(ct)
            : 0;

        return new DashboardWorkloadMetrics(
            tasks,
            workload,
            employees);
    }

    private async Task<DashboardTaskCounts> GetTaskAggregateAsync(
        IQueryable<ProfileAssignment> assignments,
        CancellationToken ct)
    {
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

                0))
            .FirstOrDefaultAsync(ct);

        return row ?? new DashboardTaskCounts(0, 0, 0, 0);
    }

    private async Task<DashboardEmployeeWorkload> GetEmployeeWorkloadAsync(
        IQueryable<ProfileAssignment> assignments,
        DashboardAccessContext context,
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
        DateTime fromUtc,
        DateTime toExclusiveUtc,
        CancellationToken ct)
    {
        var logs = uow
            .GetEntityRepository<UserProfileLogger>()
            .DbSet
            .AsNoTracking()
            .Where(log =>
                !log.IsDeleted &&
                log.ActionType ==
                    UserProfileLogConstants.ActionTypes.ProfileReviewFinalized &&
                log.CreatedDate >= fromUtc &&
                log.CreatedDate < toExclusiveUtc);

        if (context.CanViewProfileDistribution)
        {
            var employeeIds = scope
                .DistributionTeam(context)
                .Select(employee => employee.Id);

            return await logs
                .Where(log =>
                    log.PerformedById.HasValue &&
                    employeeIds.Contains(log.PerformedById.Value))
                .CountAsync(ct);
        }

        if (context.CanViewAssignedProfiles)
        {
            return await logs
                .Where(log =>
                    log.PerformedById == context.CurrentUserId)
                .CountAsync(ct);
        }

        return 0;
    }
}
