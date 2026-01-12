using Application.Recruitment.Features.Profile.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Queries;

public sealed record GetMyUserProfileQuery(Guid UserId)
    : IQuery<Result<UserProfileViewDto>>;
