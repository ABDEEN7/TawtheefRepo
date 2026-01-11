using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;

public sealed record ReviseProfileExperienceCommand(
    Guid UserId,
    SaveProfileExperienceRequest Request
) : ICommand<IResult<Unit>>;
