using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfileAchievementCommand(Guid UserId, SaveProfileAchievementRequest Request) : IRequest<IResult<Unit>>;

