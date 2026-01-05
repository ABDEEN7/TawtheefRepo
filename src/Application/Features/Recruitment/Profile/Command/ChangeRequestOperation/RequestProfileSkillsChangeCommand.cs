using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileSkillsChangeCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : ICommand<IResult<Unit>>;

