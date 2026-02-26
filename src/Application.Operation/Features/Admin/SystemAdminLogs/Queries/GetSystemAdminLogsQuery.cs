using Application.Operation.Features.Admin.SystemAdminLogs.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.SystemAdminLogs.Queries;

public sealed record GetSystemAdminLogsQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<SystemAdminLogDto>>>
{
    public Guid? UserProfileId { get; init; }
    public Guid? UserId { get; init; }
    public string? ActionType { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Search { get; init; }
}
