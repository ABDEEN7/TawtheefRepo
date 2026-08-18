using Application.Operation.Features.Employee.CandidateUsers.Contracts;
using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using Application.Operation.Features.Employee.Common.Access;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Filters;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Services;

internal sealed class CandidateUsersQueryBuilder(
    UserManager<User> userManager,
    EmployeeProfileAccessContextProvider profileAccessContextProvider,
    EmployeeProfileAccessScope profileAccessScope)
{
    public async Task<Result<IQueryable<CandidateUserListItemDto>>> BuildAsync(
        ICandidateUsersFilter filter,
        CancellationToken cancellationToken)
    {
        IQueryable<ApplicantUser> users = userManager.Users
            .OfType<ApplicantUser>()
            .AsNoTracking();

        if (filter.Scope == CandidateUsersResultScope.AccessibleProfiles)
        {
            var contextResult =
                await profileAccessContextProvider.GetAsync(cancellationToken);

            if (contextResult.IsFailed)
            {
                return Result.Fail<IQueryable<CandidateUserListItemDto>>(
                    contextResult.Errors);
            }

            var accessibleUserIds = profileAccessScope
                .AccessibleProfiles(contextResult.Value)
                .Select(profile => profile.UserId);

            users = users.Where(user =>
                accessibleUserIds.Contains(user.Id));
        }

        var search = filter.Search?.Trim();

        var hasValidYear =
            YearRange.TryCreate(filter.Year, out var yearRange);

        var profileStatuses = filter.ProfileStatuses?
            .Distinct()
            .ToArray();

        var hasProfileStatuses =
            profileStatuses is { Length: > 0 };

        var filteredUsers = users

            // Search
            .WhereIf(
                !string.IsNullOrWhiteSpace(search),
                user =>
                    EF.Functions.Like(
                        user.FullNameEn,
                        $"%{search}%") ||

                    EF.Functions.Like(
                        user.FullNameAr,
                        $"%{search}%") ||

                    (user.Email != null &&
                     EF.Functions.Like(
                         user.Email,
                         $"%{search}%")) ||

                    (user.PhoneNumber != null &&
                     EF.Functions.Like(
                         user.PhoneNumber,
                         $"%{search}%")) ||

                    (user.Profile != null &&
                     user.Profile.NationalNumber != null &&
                     EF.Functions.Like(
                         user.Profile.NationalNumber,
                         $"%{search}%")))

            // Account Status
            .WhereIf(
                filter.IsBlocked.HasValue,
                user =>
                    user.IsBlocked == filter.IsBlocked!.Value)

            // Advanced profile statuses - OR between selected values
            .WhereIf(
                hasProfileStatuses,
                user =>
                    user.Profile != null &&
                    profileStatuses!.Contains(user.Profile.Status))

            // Dashboard/deep-link status.
            // Only use it when advanced profile statuses are not selected.
            .WhereIf(
                !hasProfileStatuses &&
                filter.ProfileStatus.HasValue,
                user =>
                    user.Profile != null &&
                    user.Profile.Status ==
                    filter.ProfileStatus!.Value)

            // Year
            .WhereIf(
                hasValidYear,
                user =>
                    user.Profile != null &&
                    user.Profile.CreatedDate >= yearRange.FromUtc &&
                    user.Profile.CreatedDate < yearRange.ToExclusiveUtc);

        return Result.Ok(
            filteredUsers.Select(user =>
                new CandidateUserListItemDto
                {
                    Id = user.Id,
                    FullNameEn = user.FullNameEn,
                    FullNameAr = user.FullNameAr,
                    Email = user.EmailConfirmed
                        ? user.Email ?? string.Empty
                        : "------",
                    MobileNumber =
                        user.PhoneNumber ?? string.Empty,
                    Qid = user.Profile != null
                        ? user.Profile.NationalNumber
                        : null,
                    IsBlocked = user.IsBlocked,
                    ProfileStatus = user.Profile != null
                        ? user.Profile.Status
                        : null
                }));
    }
}
