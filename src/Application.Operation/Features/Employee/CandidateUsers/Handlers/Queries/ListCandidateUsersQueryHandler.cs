using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using Application.Operation.Features.Employee.CandidateUsers.Queries;
using Application.Operation.Features.Employee.CandidateUsers.Services;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Queries;

internal sealed class ListCandidateUsersQueryHandler(CandidateUsersQueryBuilder queryBuilder)
    : IRequestHandler<GetCandidateUsersQuery, IResult<PaginatedResult<CandidateUserListItemDto>>>
{
    public async Task<IResult<PaginatedResult<CandidateUserListItemDto>>> Handle(
        GetCandidateUsersQuery request,
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBuilder.BuildAsync(request, cancellationToken);
        if (queryResult.IsFailed)
            return Result.Fail<PaginatedResult<CandidateUserListItemDto>>(queryResult.Errors);

        var result = await queryResult.Value.ToPaginatedListAsync(request, cancellationToken);

        return Result.Ok(result);
    }
}

