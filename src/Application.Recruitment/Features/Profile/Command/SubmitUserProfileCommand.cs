using Application.Recruitment.Features.Profile.DTOs;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command;


public sealed record SubmitUserProfileCommand(
    Guid UserId,
    SubmitUserProfileRequest Request
) : ICommand<IResult<Unit>>;
