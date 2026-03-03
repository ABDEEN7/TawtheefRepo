using Application.Operation.Features.Admin.ProfileLogs.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Admin.ProfileLogs.Queries;

public sealed record GetProfileLogsQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<ProfileLogDto>>>
{
    public string? UserProfileId { get; init; }
    public string? UserId { get; init; }
    public string? Source { get; init; }
    public string? ActionType { get; init; }
    public ReviewStatus? ReviewStatus { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Search { get; init; }
}
