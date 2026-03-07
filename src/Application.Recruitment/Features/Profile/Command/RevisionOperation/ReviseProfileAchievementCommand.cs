using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileAchievementCommand(Guid UserId, SaveProfileAchievementRequest Request) : IRequest<IResult<Unit>>;

