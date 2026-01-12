using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileContactChangeCommand(
    Guid UserId,
    SaveProfileContactRequest Request
) : ICommand<IResult<Unit>>;

