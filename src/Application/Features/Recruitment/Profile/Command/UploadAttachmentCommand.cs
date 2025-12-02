using System;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record UploadAttachmentCommand(
    Guid UserId,
    Guid FileId,
    string BlobPath,
    string Hash,
    IFormFile File) : IRequest<IResult<UploadAttachmentRequest>>;
