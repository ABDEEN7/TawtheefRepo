using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.Queries;

public sealed record GetMyProfileStatusQuery(Guid UserId, ProfileSection? Section = null)
    : IRequest<Result<ProfileStatusDto>>;
