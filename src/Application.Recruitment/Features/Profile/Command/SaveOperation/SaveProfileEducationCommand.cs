using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfileEducationCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : ICommand<IResult<Unit>>;
