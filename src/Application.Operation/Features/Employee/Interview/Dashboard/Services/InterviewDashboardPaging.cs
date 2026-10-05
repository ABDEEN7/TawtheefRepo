using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Services;

internal static class InterviewDashboardPaging
{
    private const int MaxPageSize = 100;

    // The query is a flat, already-ordered SQL projection; mapping to the DTO happens after paging so
    // EF never has to translate ordering over constructor-built records.
    public static async Task<IResult<PaginatedResult<TDto>>> ToPageAsync<TRow, TDto>(
        IQueryable<TRow> orderedQuery,
        Func<TRow, TDto> map,
        InterviewDashboardPagedFilter filter,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, MaxPageSize);

        var total = await orderedQuery.CountAsync(cancellationToken);
        var rows = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Result.Ok(new PaginatedResult<TDto>(rows.Select(map).ToList(), total, pageNumber, pageSize));
    }

    public static IResult<PaginatedResult<T>> Forbidden<T>() =>
        Result.Fail<PaginatedResult<T>>(new Error(ErrorsCodes.InterviewDashboardSectionForbidden)
            .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
}
