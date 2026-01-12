using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Profile.Queries;

public sealed record GetMyProfileStatusQuery(Guid UserId, ProfileSection? Section = null)
    : IQuery<Result<ProfileStatusDto>>;
