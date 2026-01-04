using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Features.Resources.DTOs;

namespace Tawtheef.Application.Features.Resources.Commands;

public sealed record UploadAttachmentCommand(
    Guid UserId,
    Guid FileId,
    string BlobPath,
    string Hash,
    IFormFile File) : IRequest<IResult<UploadAttachmentRequest>>;
