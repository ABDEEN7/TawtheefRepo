using Application.Operation.Features.Employee.Common.Access;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Services;

internal sealed class CandidateUsersAccessScope(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    EmployeeProfileAccessContextProvider profileAccessContextProvider,
    EmployeeProfileAccessScope profileAccessScope,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<Result<CandidateUsersAccessPopulation>> GetPopulationAsync(
        CancellationToken cancellationToken)
    {
        var contextResult = await profileAccessContextProvider.GetAsync(cancellationToken);
        if (contextResult.IsFailed)
            return Result.Fail<CandidateUsersAccessPopulation>(contextResult.Errors);

        var context = contextResult.Value;
        var profiles = unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(profile => !profile.IsDeleted);
        var users = userManager.Users.OfType<ApplicantUser>().AsNoTracking();

        if (context.CurrentUser is OfficeUser { OfficeId: not null } officeUser)
        {
            profiles = profiles.Where(profile => profile.OfficeId == officeUser.OfficeId);
            var officeUserIds = profiles.Select(profile => profile.UserId);
            return Result.Ok(new CandidateUsersAccessPopulation(
                users.Where(user => officeUserIds.Contains(user.Id)),
                profiles));
        }

        if (context.CurrentUser is OfficeUser)
        {
            return Result.Ok(new CandidateUsersAccessPopulation(
                users.Where(_ => false),
                profiles.Where(_ => false)));
        }

        if (context.CurrentUser is EmployeeUser &&
            HasPermission(PermissionKeys.CandidateUsers.Manage))
            return Result.Ok(new CandidateUsersAccessPopulation(users, profiles));

        var accessibleProfileIds = profileAccessScope
            .AccessibleProfiles(context)
            .Select(profile => profile.Id);
        profiles = profiles.Where(profile => accessibleProfileIds.Contains(profile.Id));
        var accessibleUserIds = profiles.Select(profile => profile.UserId);

        return Result.Ok(new CandidateUsersAccessPopulation(
            users.Where(user => accessibleUserIds.Contains(user.Id)),
            profiles));
    }

    private bool HasPermission(string permission) =>
        httpContextAccessor.HttpContext?.User.HasClaim(RoleClaimTypes.Permission, permission) == true;
}

internal sealed record CandidateUsersAccessPopulation(
    IQueryable<ApplicantUser> Users,
    IQueryable<UserProfile> Profiles);
