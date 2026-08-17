using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using Application.Operation.Features.Employee.CandidateUsers.Queries;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Filters;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Users;

using Application.Operation.Features.Employee.CandidateUsers.Contracts;

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
        IQueryable<ApplicantUser> users = userManager.Users.OfType<ApplicantUser>().AsNoTracking();

        if (filter.Scope == CandidateUsersResultScope.AccessibleProfiles)
        {
            var contextResult = await profileAccessContextProvider.GetAsync(cancellationToken);
            if (contextResult.IsFailed)
                return Result.Fail<IQueryable<CandidateUserListItemDto>>(contextResult.Errors);

            var accessibleUserIds = profileAccessScope.AccessibleProfiles(contextResult.Value)
                .Select(profile => profile.UserId);
            users = users.Where(user => accessibleUserIds.Contains(user.Id));
        }

        var name = filter.Name?.Trim();
        var email = filter.Email?.Trim();
        var qid = filter.Qid?.Trim();
        var mobileNumber = filter.MobileNumber?.Trim();
        var hasValidYear = YearRange.TryCreate(filter.Year, out var yearRange);

        var filteredUsers = users
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                user => EF.Functions.Like(user.FullNameEn, $"%{name}%") ||
                        EF.Functions.Like(user.FullNameAr, $"%{name}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(email),
                user => user.Email != null && EF.Functions.Like(user.Email, $"%{email}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(qid),
                user => user.Profile != null && user.Profile.NationalNumber != null &&
                        EF.Functions.Like(user.Profile.NationalNumber, $"%{qid}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(mobileNumber),
                user => user.PhoneNumber != null &&
                        EF.Functions.Like(user.PhoneNumber, $"%{mobileNumber}%"))
            .WhereIf(filter.ProfileStatus.HasValue,
                user => user.Profile != null && user.Profile.Status == filter.ProfileStatus!.Value)
            .WhereIf(hasValidYear,
                user => user.Profile != null &&
                        user.Profile.CreatedDate >= yearRange.FromUtc &&
                        user.Profile.CreatedDate < yearRange.ToExclusiveUtc);

        return Result.Ok(filteredUsers.Select(user => new CandidateUserListItemDto
        {
            Id = user.Id,
            FullNameEn = user.FullNameEn,
            FullNameAr = user.FullNameAr,
            Email = user.EmailConfirmed ? (user.Email ?? string.Empty) : "------",
            MobileNumber = user.PhoneNumber ?? string.Empty,
            Qid = user.Profile != null ? user.Profile.NationalNumber : null,
            IsBlocked = user.IsBlocked,
            ProfileStatus = user.Profile != null ? user.Profile.Status : null
        }));
    }
}
