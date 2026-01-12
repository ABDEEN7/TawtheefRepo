using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfilePrereqCommand(
    Guid UserId,
    SaveProfilePrereqRequest Request
) : ICommand<IResult<Unit>>;
