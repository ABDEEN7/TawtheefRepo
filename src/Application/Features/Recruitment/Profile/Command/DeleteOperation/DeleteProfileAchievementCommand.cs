using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileAchievementCommand(Guid AchievementId): IRequest<IResult<Unit>>;