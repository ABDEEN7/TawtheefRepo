using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;
public sealed record ReviseProfileAttachmentsCommand(
    Guid UserId,
    SaveProfileAttachmentsRequest Request
) : ICommand<IResult<Unit>>;
