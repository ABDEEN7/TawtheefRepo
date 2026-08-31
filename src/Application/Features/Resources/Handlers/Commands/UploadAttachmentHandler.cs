using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Application.Features.Resources.Validation;
using Tawtheef.Domain.Entities;

namespace Tawtheef.Application.Features.Resources.Handlers.Commands;

public class UploadAttachmentHandler(
    IUnitOfWork uow,
    IFileStorageService fileStorageService)
: IRequestHandler<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>
{
    public async Task<IResult<UploadAttachmentRequest>> Handle(UploadAttachmentCommand cmd, CancellationToken ct)
    {
        var validationResult = UploadContentValidator.Validate(cmd.BlobPath, cmd.File);
        if (validationResult.IsFailed)
            return Result.Fail<UploadAttachmentRequest>(validationResult.Errors);

        await using var uploadStream = cmd.File.OpenReadStream();

        var result = await fileStorageService.SaveAsync(uploadStream, cmd.BlobPath, ct);
        if (result.IsFailed) return Result.Fail<UploadAttachmentRequest>(result.Errors);

        var safeFileName = Path.GetFileName(cmd.File.FileName).Replace("\r", "").Replace("\n", "").Trim();

        var uploadResult = await uow.GetEntityRepository<Resource>()
            .AddAsync(new Resource {
            Name        = safeFileName,
            Url         = result.Value.BlobKey,
            Key         = cmd.FileId.ToString(),
            Size        = result.Value.Size,
            Type        = cmd.File.ContentType,
            ContentHash = cmd.Hash
        }, ct);
        if (uploadResult.IsFailed) return Result.Fail<UploadAttachmentRequest>(uploadResult.Errors);
        await uow.SaveChangesAsync(ct).ConfigureAwait(false);
        return Result.Ok(new UploadAttachmentRequest(uploadResult.Value.Id, uploadResult.Value.Name));
    }
}

