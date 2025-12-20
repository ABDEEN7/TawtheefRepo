using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperations;

public sealed record DeleteProfileAchievementCommand(Guid AchievementId): IRequest<IResult<Unit>>;