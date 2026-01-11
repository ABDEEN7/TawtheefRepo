using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;


namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileAchievementCommand(Guid UserId,Guid Id): ICommand<IResult<Unit>>;
