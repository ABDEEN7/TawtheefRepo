using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileEducationCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : ICommand<IResult<Unit>>;
