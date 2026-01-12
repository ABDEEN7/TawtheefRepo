using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfilePersonalCommand(
    Guid UserId,
    SaveProfilePersonalRequest Request
) : ICommand<IResult<Unit>>;
