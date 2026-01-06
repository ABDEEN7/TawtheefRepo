using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

public sealed record SaveProfileEducationCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : ICommand<IResult<Unit>>;
