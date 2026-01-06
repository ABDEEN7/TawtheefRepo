using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;


namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileTrainingCommand(Guid TrainingId): ICommand<IResult<Unit>>;
