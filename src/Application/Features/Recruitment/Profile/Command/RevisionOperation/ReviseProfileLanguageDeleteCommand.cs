using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;


namespace Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;

public sealed record ReviseProfileLanguageDeleteCommand(Guid UserId,Guid Id): ICommand<IResult<Unit>>;
