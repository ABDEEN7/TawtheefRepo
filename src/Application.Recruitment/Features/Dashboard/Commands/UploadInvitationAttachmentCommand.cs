using Application.Recruitment.Features.Dashboard.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.Dashboard.Commands;

public record UploadInvitationAttachmentCommand(
    Guid UserId,
    Guid InvitationId,
    Guid JobRequiredAttachmentId,
    Microsoft.AspNetCore.Http.IFormFile File
) : IRequest<Result<InvitationAttachmentDto>>;
