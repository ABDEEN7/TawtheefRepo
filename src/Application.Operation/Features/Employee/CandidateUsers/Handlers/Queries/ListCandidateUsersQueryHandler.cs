using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using Application.Operation.Features.Employee.CandidateUsers.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Queries;

public sealed class ListCandidateUsersQueryHandler(UserManager<User> userManager)
    : IRequestHandler<GetCandidateUsersQuery, IResult<PaginatedResult<CandidateUserListItemDto>>>
{
    public async Task<IResult<PaginatedResult<CandidateUserListItemDto>>> Handle(
        GetCandidateUsersQuery request,
        CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var name = request.Name?.Trim();
        var email = request.Email?.Trim();
        var qid = request.Qid?.Trim();
        var mobileNumber = request.MobileNumber?.Trim();

        var queryable = userManager.Users
            .OfType<ApplicantUser>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(search),
                u => EF.Functions.Like(u.FullNameEn, $"%{search}%") ||
                     EF.Functions.Like(u.FullNameAr, $"%{search}%") ||
                     (u.Email != null && EF.Functions.Like(u.Email, $"%{search}%")) ||
                     (u.PhoneNumber != null && EF.Functions.Like(u.PhoneNumber, $"%{search}%")) ||
                     (u.Profile != null &&
                      u.Profile.NationalNumber != null &&
                      EF.Functions.Like(u.Profile.NationalNumber, $"%{search}%")))
            .WhereIf(request.IsBlocked.HasValue, u => u.IsBlocked == request.IsBlocked!.Value)
            .WhereIf(request.ProfileStatuses is { Count: > 0 },
                u => u.Profile != null && request.ProfileStatuses!.Contains(u.Profile.Status))
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                u => EF.Functions.Like(u.FullNameEn, $"%{name}%") ||
                     EF.Functions.Like(u.FullNameAr, $"%{name}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(email),
                u => u.Email != null &&
                     EF.Functions.Like(u.Email, $"%{email}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(qid),
                u => u.Profile != null &&
                     u.Profile.NationalNumber != null &&
                     EF.Functions.Like(u.Profile.NationalNumber, $"%{qid}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(mobileNumber),
                u => u.PhoneNumber != null &&
                     EF.Functions.Like(u.PhoneNumber, $"%{mobileNumber}%"));

        var result = await queryable
            .Select(u => new CandidateUserListItemDto
            {
                Id = u.Id,
                FullNameEn = u.FullNameEn,
                FullNameAr = u.FullNameAr,
                Email = u.EmailConfirmed ? (u.Email ?? string.Empty) : "------",
                MobileNumber = u.PhoneNumber ?? string.Empty,
                Qid = u.Profile != null ? u.Profile.NationalNumber : null,
                IsBlocked = u.IsBlocked,
                ProfileStatus = u.Profile != null ? u.Profile.Status : null
            })
            .ToPaginatedListAsync(request, cancellationToken);

        return Result.Ok(result);
    }
}

