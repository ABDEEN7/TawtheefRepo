using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.Dashboard.Commands;

public record DeleteInvitationAttachmentCommand(
    Guid UserId,
    Guid InvitationId,
    Guid AttachmentId
) : IRequest<Result<Unit>>;
