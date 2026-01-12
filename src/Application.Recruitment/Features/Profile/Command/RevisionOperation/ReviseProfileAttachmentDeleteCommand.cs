using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileAttachmentDeleteCommand(Guid UserId,Guid Id): ICommand<IResult<Unit>>;
