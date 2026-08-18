using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
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
        CancellationToken ct)
    {
        var tasks = await GetTaskAggregateAsync(assignments, ct);
        var workload = await GetEmployeeWorkloadAsync(assignments, awaitingDistribution, ct);
        var employees = context.CanViewProfileDistribution
            ? await scope.DistributionTeam(context).CountAsync(ct)
            : 0;
        return new DashboardWorkloadMetrics(tasks, workload, employees);
    }

    private async Task<DashboardTaskCounts> GetTaskAggregateAsync(
        IQueryable<ProfileAssignment> assignments,
        CancellationToken ct)
    {
        var changes = uow.GetEntityRepository<ProfileChangeRequest>().DbSet.AsNoTracking();
        var row = await assignments.Where(assignment =>
                assignment.IsActive && assignment.UnassignedAtUtc == null)
            .GroupBy(_ => 1)
            .Select(group => new DashboardTaskCounts(
                group.Select(assignment => assignment.UserProfileId).Distinct().Count(),
                group.Where(assignment => assignment.UserProfile!.Status == UserProfileStatus.Approved &&
                    !changes.Any(change => change.UserProfileId == assignment.UserProfileId &&
                        (change.Status == ProfileChangeRequestStatus.Pending ||
                         change.Status == ProfileChangeRequestStatus.UnderReview)))
                    .Select(assignment => assignment.UserProfileId).Distinct().Count(),
                group.Where(assignment => assignment.UserProfile!.Status != UserProfileStatus.Approved ||
                    changes.Any(change => change.UserProfileId == assignment.UserProfileId &&
                        (change.Status == ProfileChangeRequestStatus.Pending ||
                         change.Status == ProfileChangeRequestStatus.UnderReview)))
                    .Select(assignment => assignment.UserProfileId).Distinct().Count(),
                0))
            .FirstOrDefaultAsync(ct);
        return row ?? new DashboardTaskCounts(0, 0, 0, 0);
    }

    private async Task<DashboardEmployeeWorkload> GetEmployeeWorkloadAsync(
        IQueryable<ProfileAssignment> assignments,
        int awaitingDistribution,
        CancellationToken ct)
    {
        var changes = uow.GetEntityRepository<ProfileChangeRequest>()
            .DbSet
            .AsNoTracking();

        var row = await assignments
            .Where(assignment =>
                assignment.IsActive &&
                assignment.UnassignedAtUtc == null)
            .Select(assignment => new
            {
                assignment.UserProfileId,
                assignment.UserProfile!.Status,

                HasPendingChange = changes.Any(change =>
                    change.UserProfileId == assignment.UserProfileId &&
                    (change.Status == ProfileChangeRequestStatus.Pending ||
                     change.Status == ProfileChangeRequestStatus.UnderReview))
            })
            .GroupBy(_ => 1)
            .Select(group => new DashboardEmployeeWorkload(
                0,

                group
                    .Where(row =>
                        row.Status == UserProfileStatus.Submitted)
                    .Select(row => row.UserProfileId)
                    .Distinct()
                    .Count(),

                group
                    .Where(row =>
                        row.Status == UserProfileStatus.UnderReview)
                    .Select(row => row.UserProfileId)
                    .Distinct()
                    .Count(),

                group
                    .Where(row =>
                        row.Status == UserProfileStatus.Approved &&
                        row.HasPendingChange)
                    .Select(row => row.UserProfileId)
                    .Distinct()
                    .Count()))
            .FirstOrDefaultAsync(ct);

        return new DashboardEmployeeWorkload(
            awaitingDistribution,
            row?.AssignedSubmitted ?? 0,
            row?.UnderReview ?? 0,
            row?.ChangeReview ?? 0);
    }
}
