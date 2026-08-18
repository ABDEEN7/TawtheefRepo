using Application.Operation.Features.Admin.ProfileLogs.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.ProfileLogs.Queries;

public sealed record GetProfileLogsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<ProfileLogDto>>>
{
    public string? UserProfileId { get; init; }
    public string? UserId { get; init; }
    public string? Source { get; init; }
    public string? ActionType { get; init; }
    public ReviewStatus? ReviewStatus { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Search { get; init; }
    public string? CandidateSearch { get; init; }
    public string? NotesSearch { get; init; }
    public IReadOnlyCollection<string>? Sections { get; init; }
    public IReadOnlyCollection<UserProfileStatus>? ProfileStatuses { get; init; }
}

