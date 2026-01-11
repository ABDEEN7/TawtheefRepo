using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;
public sealed record ReviseProfileAttachmentsCommand(
    Guid UserId,
    SaveProfileAttachmentsRequest Request
) : ICommand<IResult<Unit>>;

public sealed record ReviseProfilePersonalAttachmentsCommand(
    Guid UserId,
    ReviseProfilePersonalAttachmentRequest Request
) : ICommand<IResult<Unit>>;
public sealed record ReviseProfilePrereqAttachmentsCommand(
    Guid UserId,
    ReviseProfilePrereqAttachmentRequest Request
) : ICommand<IResult<Unit>>;
public sealed record ReviseProfileContactAttachmentsCommand(
    Guid UserId,
    ReviseProfileContactAttachmentRequest Request
) : ICommand<IResult<Unit>>;
