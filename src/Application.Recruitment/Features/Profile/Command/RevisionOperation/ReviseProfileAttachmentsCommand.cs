using Application.Recruitment.Features.Profile.DTOs.ReviseOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;
public sealed record ReviseProfileAttachmentsCommand(
    Guid UserId,
    SaveProfileAttachmentsRequest Request
) : IRequest<IResult<Unit>>;

public sealed record ReviseProfilePersonalAttachmentsCommand(
    Guid UserId,
    ReviseProfilePersonalAttachmentRequest Request
) : IRequest<IResult<Unit>>;
public sealed record ReviseProfilePrereqAttachmentsCommand(
    Guid UserId,
    ReviseProfilePrereqAttachmentRequest Request
) : IRequest<IResult<Unit>>;
public sealed record ReviseProfileContactAttachmentsCommand(
    Guid UserId,
    ReviseProfileContactAttachmentRequest Request
) : IRequest<IResult<Unit>>;

