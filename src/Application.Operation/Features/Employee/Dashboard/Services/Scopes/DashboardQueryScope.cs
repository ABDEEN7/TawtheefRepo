using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Scopes;

internal sealed class DashboardQueryScope(
    IUnitOfWork uow,
    UserManager<User> userManager)
{
    public IQueryable<UserProfile> Profiles(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DashboardDateRange range)
    {
        var assignments = uow.GetEntityRepository<ProfileAssignment>().DbSet;
        var query = AccessibleProfiles(context);

        query = query.Where(x => x.CreatedDate >= range.FromUtc && x.CreatedDate < range.ToExclusiveUtc);
        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.TargetEntityId == request.DepartmentId);
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<UserProfileStatus>(request.Status, true, out var status))
            query = query.Where(x => x.Status == status);

        if (context.Scope == DashboardScope.User)
            return ApplyActiveAssignmentScope(query, assignments, context.CurrentUserId);

        if (!request.EmployeeId.HasValue) return query;

        var allowedEmployees = DashboardEmployees(context).Select(user => user.Id);
        return ApplyActiveAssignmentScope(
            query,
            assignments.Where(assignment => allowedEmployees.Contains(assignment.EmployeeId)),
            request.EmployeeId.Value);
    }

    public IQueryable<UserProfile> ProfilesForPeriod(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DateTime from,
        DateTime toExclusive) =>
        Profiles(request, context, new DashboardDateRange(from, toExclusive));

    public IQueryable<UserProfile> AccessibleProfiles(DashboardAccessContext context)
    {
        var profiles = uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(profile => !profile.IsDeleted);

        if (context.Scope == DashboardScope.Organization) return profiles;

        if (context.Scope == DashboardScope.Office && context.CurrentUser is OfficeUser { OfficeId: not null } officeUser)
            return profiles.Where(profile => profile.OfficeId == officeUser.OfficeId);

        var assignments = uow.GetEntityRepository<ProfileAssignment>().DbSet;
        return profiles.Where(profile => assignments.Any(assignment =>
            !assignment.IsDeleted &&
            assignment.UserProfileId == profile.Id &&
            assignment.EmployeeId == context.CurrentUserId &&
            assignment.IsActive &&
            assignment.UnassignedAtUtc == null));
    }

    public IQueryable<Job> Jobs(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DashboardDateRange range)
    {
        var query = AccessibleJobs(context);
        query = query.Where(x => x.CreatedDate >= range.FromUtc && x.CreatedDate < range.ToExclusiveUtc);
        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == request.DepartmentId);
        return query;
    }

    public IQueryable<Job> JobsForPeriod(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DateTime from,
        DateTime toExclusive) =>
        Jobs(request, context, new DashboardDateRange(from, toExclusive));

    public IQueryable<Job> AccessibleJobs(DashboardAccessContext context)
    {
        var jobs = uow.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .Where(job => !job.IsDeleted);

        if (context.Scope == DashboardScope.Organization) return jobs;

        if (context.Scope == DashboardScope.Office)
        {
            var employeeIds = DashboardEmployees(context).Select(user => user.Id);
            return jobs.Where(job => job.CreatedById.HasValue && employeeIds.Contains(job.CreatedById.Value));
        }

        return jobs.Where(job => job.CreatedById == context.CurrentUserId);
    }

    public IQueryable<ProfileAssignment> Assignments(
        DashboardAccessContext context,
        Guid? requestedEmployeeId = null)
    {
        var allowedProfiles = AccessibleProfiles(context).Select(profile => profile.Id);
        var allowedEmployees = DashboardEmployees(context).Select(user => user.Id);
        var query = uow.GetEntityRepository<ProfileAssignment>().DbSet
            .AsNoTracking()
            .Where(assignment =>
                !assignment.IsDeleted &&
                allowedProfiles.Contains(assignment.UserProfileId) &&
                allowedEmployees.Contains(assignment.EmployeeId));

        var effectiveEmployeeId = context.Scope == DashboardScope.User
            ? context.CurrentUserId
            : requestedEmployeeId;

        return effectiveEmployeeId.HasValue
            ? query.Where(assignment => assignment.EmployeeId == effectiveEmployeeId.Value)
            : query;
    }

    public IQueryable<User> DashboardEmployees(DashboardAccessContext context)
    {
        var users = userManager.Users.AsNoTracking()
            .Where(user => !user.IsBlocked && !user.IsDeleted);

        return context.Scope switch
        {
            DashboardScope.Organization => users.Where(user =>
                user is EmployeeUser || user is OfficeUser),
            DashboardScope.Office when context.CurrentUser is OfficeUser { OfficeId: not null } officeUser =>
                users.OfType<OfficeUser>()
                    .Where(user => user.OfficeId == officeUser.OfficeId)
                    .Cast<User>(),
            _ => users.Where(user => user.Id == context.CurrentUserId)
        };
    }

    public IQueryable<UserProfileLogger> CompletedReviews(
        DashboardAccessContext context,
        Guid? requestedEmployeeId = null)
    {
        var allowedProfiles = AccessibleProfiles(context).Select(profile => profile.Id);
        var allowedEmployees = DashboardEmployees(context).Select(user => user.Id);
        var effectiveEmployeeId = context.Scope == DashboardScope.User
            ? context.CurrentUserId
            : requestedEmployeeId;

        var query = uow.GetEntityRepository<UserProfileLogger>().DbSet
            .AsNoTracking()
            .Where(log =>
                !log.IsDeleted &&
                allowedProfiles.Contains(log.UserProfileId) &&
                log.PerformedById.HasValue &&
                allowedEmployees.Contains(log.PerformedById.Value));

        return effectiveEmployeeId.HasValue
            ? query.Where(log => log.PerformedById == effectiveEmployeeId.Value)
            : query;
    }

    public IQueryable<Invitation> Invitations(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DashboardDateRange range)
    {
        var allowedJobs = Jobs(request, context, range).Select(job => job.Id);
        return uow.GetEntityRepository<Invitation>().DbSet.AsNoTracking()
            .Where(invitation => !invitation.IsDeleted && allowedJobs.Contains(invitation.JobId));
    }

    private static IQueryable<UserProfile> ApplyActiveAssignmentScope(
        IQueryable<UserProfile> profiles,
        IQueryable<ProfileAssignment> assignments,
        Guid employeeId) =>
        profiles.Where(profile => assignments.Any(assignment =>
            !assignment.IsDeleted &&
            assignment.UserProfileId == profile.Id && assignment.EmployeeId == employeeId &&
            assignment.IsActive && assignment.UnassignedAtUtc == null));
}
