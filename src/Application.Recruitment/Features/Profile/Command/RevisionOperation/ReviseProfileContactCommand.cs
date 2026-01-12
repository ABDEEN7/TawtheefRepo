using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileContactCommand(
    Guid UserId,
    SaveProfileContactRequest Request
) : ICommand<IResult<Unit>>;
