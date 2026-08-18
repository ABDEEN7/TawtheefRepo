using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Scopes;

internal sealed class DashboardQueryScope(
    IUnitOfWork uow,
    UserManager<User> userManager,
    EmployeeProfileAccessScope profileAccessScope)
{
    public IQueryable<UserProfile> Profiles(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DashboardDateRange range)
    {
        var assignments = uow.GetEntityRepository<ProfileAssignment>().DbSet;
        var query = AccessibleProfiles(context);

        query = query.Where(x => x.CreatedDate >= range.FromUtc && x.CreatedDate < range.ToExclusiveUtc);
        if (request.DepartmentId.HasValue) query = query.Where(x => x.TargetEntityId == request.DepartmentId);
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<UserProfileStatus>(request.Status, true, out var status))
            query = query.Where(x => x.Status == status);

        if (!request.EmployeeId.HasValue) return query;

        var employeeId = request.EmployeeId.Value;
        if (context.CanViewProfileDistribution)
        {
            var allowedEmployees = DistributionTeam(context).Select(user => user.Id);
            return ApplyActiveAssignmentScope(
                query,
                assignments.Where(assignment => allowedEmployees.Contains(assignment.EmployeeId)),
                employeeId);
        }

        return employeeId == context.CurrentUserId
            ? query
            : query.Where(_ => false);
    }

    public IQueryable<UserProfile> ProfilesForPeriod(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DateTime from,
        DateTime toExclusive) =>
        Profiles(request, context, new DashboardDateRange(from, toExclusive));

    public IQueryable<UserProfile> AccessibleProfiles(DashboardAccessContext context)
        => profileAccessScope.AccessibleProfiles(new EmployeeProfileAccessContext(
            context.CurrentUserId,
            context.CurrentUser,
            context.CanViewProfileDistribution,
            context.CanViewAssignedProfiles));

    public IQueryable<Job> Jobs(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DashboardDateRange range)
    {
        var query = AccessibleJobs(context);
        query = query.Where(x => x.CreatedDate >= range.FromUtc && x.CreatedDate < range.ToExclusiveUtc);
        if (request.DepartmentId.HasValue) query = query.Where(x => x.DepartmentId == request.DepartmentId);
        return query;
    }

    public IQueryable<Job> JobsForPeriod(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DateTime from,
        DateTime toExclusive) =>
        Jobs(request, context, new DashboardDateRange(from, toExclusive));

    public IQueryable<Job> AccessibleJobs(DashboardAccessContext context) =>
        uow.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .Where(job => !job.IsDeleted && context.CanViewJobs)
            .ApplyJobAccessScope(new EmployeeJobAccessContext(
                context.CurrentUserId,
                context.HasFullJobAccess));

    public IQueryable<ProfileAssignment> Assignments(DashboardAccessContext context)
    {
        var allowedProfiles = AccessibleProfiles(context).Select(profile => profile.Id);
        var query = uow.GetEntityRepository<ProfileAssignment>().DbSet
            .AsNoTracking()
            .Where(assignment => !assignment.IsDeleted && allowedProfiles.Contains(assignment.UserProfileId));
        if (context.CanViewProfileDistribution)
        {
            var allowedEmployees = DistributionTeam(context).Select(user => user.Id);
            return query.Where(assignment => allowedEmployees.Contains(assignment.EmployeeId));
        }

        return context.CanViewAssignedProfiles
            ? query.Where(assignment => assignment.EmployeeId == context.CurrentUserId)
            : query.Where(_ => false);
    }

    public IQueryable<User> DistributionTeam(DashboardAccessContext context)
    {
        var reviewerRoles = uow.Context.Set<IdentityRoleClaim<Guid>>()
            .AsNoTracking()
            .Where(claim => claim.ClaimType == RoleClaimTypes.Permission &&
                            claim.ClaimValue == PermissionKeys.ProfileApproval.Review)
            .Select(claim => claim.RoleId);
        var reviewers = uow.Context.Set<IdentityUserRole<Guid>>()
            .AsNoTracking()
            .Where(userRole => reviewerRoles.Contains(userRole.RoleId))
            .Select(userRole => userRole.UserId);

        return ApplyTeamMembership(userManager.Users.AsNoTracking(), context.CurrentUser)
            .Where(user => reviewers.Contains(user.Id) && !user.IsBlocked && !user.IsDeleted);
    }

    public IQueryable<Invitation> Invitations(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DashboardDateRange range)
    {
        var allowedJobs = Jobs(request, context, range).Select(job => job.Id);
        return uow.GetEntityRepository<Invitation>().DbSet.AsNoTracking()
            .Where(invitation => context.CanViewInvitations && !invitation.IsDeleted && allowedJobs.Contains(invitation.JobId));
    }

    private static IQueryable<User> ApplyTeamMembership(IQueryable<User> query, User currentUser) =>
        currentUser switch
        {
            OfficeUser { OfficeId: not null } officeUser => query.OfType<OfficeUser>()
                .Where(user => user.OfficeId == officeUser.OfficeId).Cast<User>(),
            EmployeeUser => query.OfType<EmployeeUser>().Cast<User>(),
            _ => query.Where(_ => false)
        };

    private static IQueryable<UserProfile> ApplyActiveAssignmentScope(
        IQueryable<UserProfile> profiles,
        IQueryable<ProfileAssignment> assignments,
        Guid employeeId) =>
        profiles.Where(profile => assignments.Any(assignment =>
            assignment.UserProfileId == profile.Id && assignment.EmployeeId == employeeId &&
            assignment.IsActive && assignment.UnassignedAtUtc == null));
}
