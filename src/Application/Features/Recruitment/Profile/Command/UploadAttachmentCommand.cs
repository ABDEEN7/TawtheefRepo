using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record UploadAttachmentCommand(IFormFile file) : IRequest<IResult<UploadAttachmentRequest>>;
