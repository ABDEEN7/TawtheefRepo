using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfileContactCommand(
    Guid UserId,
    SaveProfileContactRequest Request
) : ICommand<IResult<Unit>>;
