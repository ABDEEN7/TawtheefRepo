using Application.Operation.Features.Admin.ProfileLogs.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.CandidateUsers.Queries;

public sealed record GetCandidateUserProfileLogsQuery
    : PaginatedRequest, IRequest<IResult<PaginatedResult<ProfileLogDto>>>
{
    public Guid UserId { get; init; }
}
