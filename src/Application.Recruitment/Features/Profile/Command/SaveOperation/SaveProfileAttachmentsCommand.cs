using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;
public sealed record SaveProfileAttachmentsCommand(
    Guid UserId,
    SaveProfileAttachmentsRequest Request
) : ICommand<IResult<Unit>>;
