using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

public sealed record SaveProfileAchievementCommand(Guid UserId, SaveProfileAchievementRequest Request) : IRequest<IResult<Unit>>;
