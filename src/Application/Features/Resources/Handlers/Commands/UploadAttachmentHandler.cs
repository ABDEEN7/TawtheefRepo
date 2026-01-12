using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;

namespace Tawtheef.Application.Features.Resources.Handlers.Commands;

public class UploadAttachmentHandler(
    IUnitOfWork uow,
    IFileStorageService fileStorageService)
: ICommandHandler<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>
{
    public async Task<IResult<UploadAttachmentRequest>> Handle(UploadAttachmentCommand cmd, CancellationToken ct)
    {
        if (cmd.File.Length == 0) return Result.Fail<UploadAttachmentRequest>(ErrorsCodes.EmptyFile);

        await using var uploadStream = cmd.File.OpenReadStream();

        var result = await fileStorageService.SaveAsync(uploadStream, cmd.BlobPath, ct);
        if (result.IsFailed) return Result.Fail<UploadAttachmentRequest>(result.Errors);

        var uploadResult = await uow.GetEntityRepository<Resource>().AddAsync(new Resource
        {
            Name        = cmd.File.FileName,
            Url         = result.Value.BlobKey,
            Key         = cmd.FileId.ToString(),
            Size        = result.Value.Size,
            Type        = cmd.File.ContentType,
            ContentHash = cmd.Hash
        });
        if (uploadResult.IsFailed) return Result.Fail<UploadAttachmentRequest>(uploadResult.Errors);
        await uow.SaveChangesAsync(ct).ConfigureAwait(false);
        return Result.Ok(new UploadAttachmentRequest(uploadResult.Value.Id, uploadResult.Value.Name));
    }
}
