using Application.Operation.Features.Employee.Interview.Committee.DTOs;
using Application.Operation.Features.Employee.Interview.Committee.Queries;
using Application.Operation.Features.Employee.Interview.Committee.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Extensions;

namespace Application.Operation.Features.Employee.Interview.Committee.Handlers.Queries;

public sealed class ListEligibleCommitteeMembersQueryHandler(
    CommitteeMemberEligibilityService eligibility,
    ILocalizationService localization)
    : IRequestHandler<ListEligibleCommitteeMembersQuery, IResult<List<EligibleCommitteeMemberDto>>>
{
    // The dropdown is searchable so we limit the results to enhance performance.
    private const int MaxResults = 50;

    public async Task<IResult<List<EligibleCommitteeMemberDto>>> Handle(
        ListEligibleCommitteeMembersQuery request, CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var hasSearch = !string.IsNullOrEmpty(search);

        var users = (await eligibility.GetEligibleUsersAsync(cancellationToken))
            .WhereIf(hasSearch, u =>
                u.FullNameAr.Contains(search!) ||
                u.FullNameEn.Contains(search!) ||
                (u.Email != null && u.Email.Contains(search!)));

        var isArabic = string.Equals(localization.GetCurrentLanguage(), "ar", StringComparison.OrdinalIgnoreCase);
        var ordered = isArabic ? users.OrderBy(u => u.FullNameAr) : users.OrderBy(u => u.FullNameEn);

        var members = await ordered
            .Take(MaxResults)
            .Select(u => new EligibleCommitteeMemberDto(u.Id, u.FullNameAr, u.FullNameEn, u.Email))
            .ToListAsync(cancellationToken);

        return Result.Ok(members);
    }
}
