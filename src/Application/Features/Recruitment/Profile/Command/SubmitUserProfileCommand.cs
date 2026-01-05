using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;


public sealed record SubmitUserProfileCommand(
    Guid UserId,
    SubmitUserProfileRequest Request
) : ICommand<IResult<Unit>>;
