using Application.Recruitment.Features.Profile.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Queries;

public sealed record GetMyUserProfileSummaryQuery(Guid UserId)
    : IQuery<Result<UserProfileSummaryDto>>;
