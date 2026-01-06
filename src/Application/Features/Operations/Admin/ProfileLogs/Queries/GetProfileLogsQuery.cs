using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.ProfileLogs.DTOs;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Admin.ProfileLogs.Queries;

public sealed record GetProfileLogsQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<ProfileLogDto>>>
{
    public Guid? UserProfileId { get; init; }
    public Guid? UserId { get; init; }
    public string? Source { get; init; }
    public string? ActionType { get; init; }
    public ReviewStatus? ReviewStatus { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Search { get; init; }
}
