using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileSkillsChangeCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : ICommand<IResult<Unit>>;

