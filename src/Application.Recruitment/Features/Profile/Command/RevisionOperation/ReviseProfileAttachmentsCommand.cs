using Application.Recruitment.Features.Profile.DTOs.ReviseOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;
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
