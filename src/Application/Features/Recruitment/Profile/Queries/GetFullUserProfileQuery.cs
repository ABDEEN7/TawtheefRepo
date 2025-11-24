using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Queries;

public sealed record GetFullUserProfileQuery(Guid UserId): IRequest<IResult<GetUserProfileDto>>;
