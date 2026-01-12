using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileEducationChangeCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : ICommand<IResult<Unit>>;

