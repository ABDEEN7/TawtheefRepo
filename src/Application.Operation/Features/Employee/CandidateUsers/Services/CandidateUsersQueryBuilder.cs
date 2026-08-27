using Application.Operation.Features.Employee.CandidateUsers.Contracts;
using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Filters;
using Tawtheef.Application.Extensions;

namespace Application.Operation.Features.Employee.CandidateUsers.Services;

internal sealed class CandidateUsersQueryBuilder(CandidateUsersAccessScope accessScope)
{
    public async Task<Result<IQueryable<CandidateUserListItemDto>>> BuildAsync(
        ICandidateUsersFilter filter,
        CancellationToken cancellationToken)
    {
        var populationResult = await accessScope.GetPopulationAsync(cancellationToken);
        if (populationResult.IsFailed)
            return Result.Fail<IQueryable<CandidateUserListItemDto>>(populationResult.Errors);

        var population = populationResult.Value;
        var users = population.Users;

        // Request scope may narrow the server-authorized population, never widen it.
        if (filter.Scope == CandidateUsersResultScope.AccessibleProfiles)
        {
            var authorizedProfileUserIds = population.Profiles.Select(profile => profile.UserId);
            users = users.Where(user => authorizedProfileUserIds.Contains(user.Id));
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
